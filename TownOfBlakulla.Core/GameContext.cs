using System;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core
{
    public class GameContext
    {
        public GameState State { get; set; }
        public int PhaseIndex { get; set; }
        public int SubPhaseIndex { get; set; }
        public int PlayerCount { get; set; }
        public DateTime StartTime { get; set; }
    }
}