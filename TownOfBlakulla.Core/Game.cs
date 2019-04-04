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
        private readonly IActionQueue actionQueue;
        private readonly GameContext context;
        private readonly object mutex = new object();

        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<GameActionReply>> messageLookup
            = new ConcurrentDictionary<Guid, TaskCompletionSource<GameActionReply>>();

        public Game(
            ILogger logger,
            IPlayerHandler playerHandler,
            IActionQueue actionQueue)
        {
            this.logger = logger;
            this.playerHandler = playerHandler;
            this.actionQueue = actionQueue;
            this.context = new GameContext();
        }

        public void Update(UpdateGameStateRequest request)
        {
            lock (mutex)
            {
                context.SubPhaseIndex = request.SubPhaseIndex;
                context.PhaseIndex = request.PhaseIndex;
                context.PlayerCount = request.PlayerCount;
                context.StartTime = request.StartTime;
                context.State = request.State;

                if (request.Messages != null && request.Messages.Count > 0)
                {
                    this.HandleMessages(request.Messages);
                }
            }
        }

        public GameStateResponse GetState(TwitchViewer viewerContext)
        {
            lock (mutex)
            {
                return new GameStateResponse(
                    context.State,
                    playerHandler.IsPlaying(viewerContext));
            }
        }

        public Task<LeaveResponse> LeaveAsync(TwitchViewer viewerContext)
        {
            return AwaitResponse<LeaveResponse>("leave");
        }

        public Task<JoinResponse> JoinAsync(TwitchViewer viewerContext, string name)
        {
            return AwaitResponse<JoinResponse>("join", name);
        }

        public Task<VoteResponse> VoteAsync(TwitchViewer viewerContext, string value)
        {
            return AwaitResponse<VoteResponse>("vote");
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

        private async Task<T> AwaitResponse<T>(string name, string arguments = null)
        {
            return await AwaitResponse<T>(Guid.NewGuid(), name, arguments);
        }

        private async Task<T> AwaitResponse<T>(Guid id, string name, string arguments)
        {
            var taskSource = new TaskCompletionSource<GameActionReply>();

            messageLookup[id] = taskSource;

            actionQueue.Enqueue(new GameAction(id, name, arguments));

            var result = await taskSource.Task;

            return JsonConvert.DeserializeObject<T>(result.Data);
        }
    }
}