namespace TownOfBlakulla.Core.Models
{
    public class Role
    {
        public Role(string name, string alignment, string summary, string abilities, string attribute)
        {
            Name = name;
            Alignment = alignment;
            Summary = summary;
            Abilities = abilities;
            Attribute = attribute;
        }

        public string Name { get; }
        public string Alignment { get; }
        public string Summary { get; }
        public string Abilities { get; }
        public string Attribute { get; }
    }
}