namespace TownOfBlakulla.Core.Models
{
    public class PlayerInfo
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public Role Role { get; set; }

        public bool Lynched { get; set; }
        public bool Blackmailed { get; set; }
        public bool Cleaned { get; set; }
        public bool TargetByMafioso { get; set; }
        public bool TargetByGodfather { get; set; }
        public bool Healed { get; set; }
        public bool Jailed { get; set; }
        public bool RevealedAsMayor { get; set; }
    }
}