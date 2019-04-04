using System;

namespace TownOfBlakulla.Core.Models
{
    public class GameActionReply
    {
        public GameActionReply(Guid correlationId, string data)
        {
            CorrelationId = correlationId;
            Data = data;
        }

        public Guid CorrelationId { get; }

        public string Data { get; }
    }
}