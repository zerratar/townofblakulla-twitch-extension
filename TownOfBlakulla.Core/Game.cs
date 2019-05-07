using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TownOfBlakulla.Core.Handlers;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core
{
    public class Game : IGame
    {
        private readonly ILogger logger;
        private readonly IPlayerHandler playerHandler;
        private readonly IChatHandler chatHandler;
        private readonly IActionQueue actionQueue;
        private readonly IPropertyRepository propertyRepository;

        private readonly GameContext context;
        private readonly object mutex = new object();

        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<GameActionReply>> messageLookup;

        private GameState lastState = GameState.NotStarted;

        public Game(
            ILogger logger,
            IPlayerHandler playerHandler,
            IChatHandler chatHandler,
            IActionQueue actionQueue,
            IPropertyRepository propertyRepository)
        {
            this.logger = logger;
            this.playerHandler = playerHandler;
            this.chatHandler = chatHandler;
            this.actionQueue = actionQueue;
            this.propertyRepository = propertyRepository;

            this.messageLookup = propertyRepository.Load<ConcurrentDictionary<Guid, TaskCompletionSource<GameActionReply>>>(nameof(messageLookup))
                                 ?? new ConcurrentDictionary<Guid, TaskCompletionSource<GameActionReply>>();

            this.context = propertyRepository.Load<GameContext>(nameof(context)) ?? GameContext.CreateEmpty();
        }

        public void PushMessages(MessageRequest request)
        {
            if (request.Messages != null && request.Messages.Count > 0)
            {
                this.HandleMessages(request.Messages);
            }
        }

        public void Update(UpdateGameStateRequest request)
        {
            lock (mutex)
            {
                context.GameState = request.GameState;

                propertyRepository.Save(nameof(context), context);

                playerHandler.SetActivePlayers(context.GameState?.Players);
            }
        }

        public GameStateResponse GetState(TwitchViewer viewerContext)
        {
            lock (mutex)
            {
                var state =
                    context.GameState == null
                        ? GameState.NotStarted
                        : context.GameState.Joinable
                            ? GameState.Joinable
                            : context.GameState.Started
                                ? GameState.Started
                                : GameState.NotStarted;

                if (state != this.lastState && state == GameState.NotStarted)
                {
                    ResetGame();
                }

                var lynched = false;
                var abilityArguments = new string[0];
                var player = this.playerHandler.Get(viewerContext);
                if (player != null)
                {
                    lynched = player.Lynched;
                    abilityArguments = player.Role.GetUsableArguments(player, this.context);
                }

                var isPlaying = playerHandler.IsPlaying(viewerContext);
                this.lastState = state;
                return new GameStateResponse(
                    state,
                    isPlaying,
                    lynched,
                    abilityArguments,
                    GameInfo.FromUpdateInfo(context.GameState, isPlaying),
                    context.Revision);
            }
        }

        public async Task<GameStateResponse> GetStateAsync(TwitchViewer viewerContext, int revision)
        {
            var timeout = 0;
            var state = this.GetState(viewerContext);
            while (state.Revision == revision)
            {
                await Task.Delay(50);
                timeout += 50;
                state = this.GetState(viewerContext);
                if (timeout >= 20_000) break;
            }


            return state;
        }

        public async Task<LeaveResponse> LeaveAsync(TwitchViewer viewerContext)
        {
            var result = await AwaitResponse<LeaveResponse>("leave", viewerContext.Identifier);
            if (result != null)
            {
                playerHandler.Remove(viewerContext);
            }

            return result;
        }

        public async Task<JoinResponse> JoinAsync(TwitchViewer viewerContext, string name)
        {
            var result = await AwaitResponse<JoinResponse>("join", viewerContext.Identifier, name);
            if (result != null && !string.IsNullOrEmpty(result.Role))
            {
                result.Game = GameInfo.FromUpdateInfo(context.GameState, true);
                playerHandler.Add(viewerContext, name);
            }
            return result;
        }

        public Task<VoteResponse> VoteAsync(TwitchViewer viewerContext, string value)
        {
            return AwaitResponse<VoteResponse>("vote", viewerContext.Identifier, value);
        }

        public Task<ChatMessage> SendChatMessageAsync(TwitchViewer viewerContext, string channel, string message)
        {
            return AwaitChatResponse(viewerContext.Identifier, channel, message);
        }

        public Task<LastWillResponse> UpdateLastWill(TwitchViewer viewerContext, string lastWill)
        {
            return AwaitResponse<LastWillResponse>("last-will", viewerContext.Identifier, lastWill);
        }

        public Task<DeathNoteResponse> UpdateDeathNote(TwitchViewer viewerContext, string deathNote)
        {
            return AwaitResponse<DeathNoteResponse>("death-note", viewerContext.Identifier, deathNote);
        }

        public Task<UseAbilityResponse> UseAbility(TwitchViewer viewerContext, string arguments)
        {
            return AwaitResponse<UseAbilityResponse>("use-ability", viewerContext.Identifier, arguments);
        }

        private void HandleMessages(IReadOnlyList<GameActionReply> requestMessages)
        {
            foreach (var msg in requestMessages)
            {
                HandleMessage(msg);
            }
        }

        private void HandleMessage(GameActionReply message)
        {
            if (!messageLookup.TryGetValue(message.CorrelationId, out var task))
            {
                this.logger.Error(
                    $"HandleMessage received unhandled message with correlation id: {message.CorrelationId} and data: {message.Data}");
                return;
            }

            task.SetResult(message);
            messageLookup.Remove(message.CorrelationId, out _);
        }

        private void ResetGame()
        {
            lock (mutex)
            {
                playerHandler.Reset();

            }
        }

        private async Task<ChatMessage> AwaitChatResponse(params string[] arguments)
        {
            var chatResponse = await AwaitResponse<ChatResponse>("chat", arguments);

            return chatHandler.HandleChatResponse(chatResponse);
        }

        private async Task<T> AwaitResponse<T>(string name, params string[] arguments)
        {
            return await AwaitResponse<T>(Guid.NewGuid(), name, arguments);
        }

        private async Task<T> AwaitResponse<T>(Guid id, string name, params string[] arguments)
        {
            var taskSource = new TaskCompletionSource<GameActionReply>();

            messageLookup[id] = taskSource;

            actionQueue.Enqueue(new GameAction(id, name, arguments));

            var result = await taskSource.Task;

            return JsonConvert.DeserializeObject<T>(result.Data);
        }
    }
}