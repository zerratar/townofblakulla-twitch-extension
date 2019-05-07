namespace TownOfBlakulla.Core.Models
{
    public class SubPhaseInfo
    {
        public string Name { get; set; }
        public float Timer { get; set; }
        public float Duration { get; set; }
        public float EnterTime { get; set; }
        public float ExitTime { get; set; }
        public bool HasEnded { get; set; }
        public bool IsActive { get; set; }
    }
}