namespace TownOfBlakulla.Core.Models
{
    public class JoinGameRequest
    {
        public JoinGameRequest(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}