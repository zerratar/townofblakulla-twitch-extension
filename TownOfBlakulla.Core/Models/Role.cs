namespace TownOfBlakulla.Core.Models
{
    public class Role
    {
        public Role(string name, string alignment, string summary, string abilities)
        {
            Name = name;
            Alignment = alignment;
            Summary = summary;
            Abilities = abilities;
        }

        public string Name { get; }
        public string Alignment { get; }
        public string Summary { get; }
        public string Abilities { get; }
    }
}