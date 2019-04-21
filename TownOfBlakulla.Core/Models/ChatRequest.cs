namespace TownOfBlakulla.Core.Models
{
    public class ChatRequest
    {
        public ChatRequest(string channel, string message)
        {
            Channel = channel;
            Message = message;
        }

        public string Channel { get; }
        public string Message { get; }
    }
}