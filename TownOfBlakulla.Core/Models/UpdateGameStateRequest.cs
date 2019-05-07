using System;

namespace TownOfBlakulla.Core.Models
{
    public class UpdateGameStateRequest
    {
        public UpdateGameStateRequest(GameUpdateInfo gameState)
        {
            GameState = gameState;
        }

        public GameUpdateInfo GameState { get; }
    }
}