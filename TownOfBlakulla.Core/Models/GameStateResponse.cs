namespace TownOfBlakulla.Core.Models
{
    public class GameStateResponse
    {
        public GameStateResponse(GameState state, bool hasJoined)
        {
            State = state;
            HasJoined = hasJoined;
        }

        public bool HasJoined { get; }
        public GameState State { get; }
    }
}