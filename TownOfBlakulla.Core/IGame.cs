using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core
{
    public interface IGame
    {
        void Update(UpdateGameStateRequest request);

        GameStateResponse GetState(TwitchViewer viewerContext);
    }
}