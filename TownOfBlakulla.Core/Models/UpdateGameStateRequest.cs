using System;

namespace TownOfBlakulla.Core.Models
{
    public class UpdateGameStateRequest
    {
        public UpdateGameStateRequest(
            GameState state, 
            int phaseIndex, 
            int subPhaseIndex, 
            int playerCount, 
            DateTime startTime)
        {
            State = state;
            PhaseIndex = phaseIndex;
            SubPhaseIndex = subPhaseIndex;
            PlayerCount = playerCount;
            StartTime = startTime;
        }

        public GameState State { get; }
        public int PhaseIndex { get; }
        public int SubPhaseIndex { get; }
        public int PlayerCount { get; }
        public DateTime StartTime { get; }
    }
}