using System;
using System.Collections.Generic;

namespace TownOfBlakulla.Core.Models
{
    public class PlayerInfo
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public bool Lynched { get; set; }
        public Role Role { get; set; }
    }

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

    public class SubPhaseInfo
    {
        public string Name { get; set; }
        public float Timer { get; set; }
        public float Duration { get; set; }
        public DateTime EnterTime { get; set; }
        public DateTime ExitTime { get; set; }
        public bool HasEnded { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateGameStateRequest
    {
        public UpdateGameStateRequest(GameUpdateInfo gameState)
        {
            GameState = gameState;
        }

        public GameUpdateInfo GameState { get; }
    }

    public class MessageRequest
    {
        public MessageRequest(IReadOnlyList<GameActionReply> messages)
        {
            this.Messages = messages;
        }
        public IReadOnlyList<GameActionReply> Messages { get; }
    }
}