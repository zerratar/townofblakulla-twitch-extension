namespace TownOfBlakulla.Core.Models
{
    public class GameAction
    {
        public GameAction(string name, string arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public string Name { get; }

        public string Arguments { get; }

        public static GameAction None { get; } = new GameAction(null, null);
    }
}