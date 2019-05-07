using System.Collections.Generic;

namespace TownOfBlakulla.Core.Models
{
    public class GameUpdateInfo
    {
        public GameUpdateInfo()
        {
            SubPhase = new SubPhaseInfo();
            Players = new List<PlayerInfo>();
        }

        public string PhaseName { get; set; }
        public SubPhaseInfo SubPhase { get; set; }
        public int DaysPassed { get; set; }
        public bool Started { get; set; }
        public bool Joinable { get; set; }
        public List<PlayerInfo> Players { get; set; }
    }
}