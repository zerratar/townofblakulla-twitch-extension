using System.Collections.Generic;

namespace TownOfBlakulla.Core.Models
{
    public class MessageRequest
    {
        public MessageRequest(IReadOnlyList<GameActionReply> messages)
        {
            this.Messages = messages;
        }
        public IReadOnlyList<GameActionReply> Messages { get; }
    }
}