namespace TownOfBlakulla.Core.Models
{    
    public class Player
    {
        public Player(int id, string name, Role role)
        {
            Id = id;
            Name = name;
            Role = role;
        }

        public int Id { get; }
        public string Name { get; }
        public Role Role { get; }
    }
}