using System;

namespace TownOfBlakulla.Core.Models
{
    public class GameAction
    {
        public GameAction(Guid correlationId, string name, string arguments)
        {
            CorrelationId = correlationId;
            Name = name;
            Arguments = arguments;
        }

        public Guid CorrelationId { get; }

        public string Name { get; }

        public string Arguments { get; }

        public static GameAction None { get; } = new GameAction(Guid.Empty, null, null);
    }
}