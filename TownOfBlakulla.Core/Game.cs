using TownOfBlakulla.Core.Handlers;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core
{
    public class Game : IGame
    {
        private readonly IPlayerHandler playerHandler;
        private readonly IActionQueue actionQueue;
        private readonly GameContext context;
        private readonly object mutex = new object();

        public Game(
            IPlayerHandler playerHandler,
            IActionQueue actionQueue)
        {
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
    }
}