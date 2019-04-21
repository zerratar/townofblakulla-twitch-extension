namespace TownOfBlakulla.Core.Models
{
    public class ChatResponse
    {
        public ChatResponse(string username, string channel, string message)
        {
            Username = username;
            Channel = channel;
            Message = message;
        }

        public string Username { get; }
        public string Channel { get; }        
        public string Message { get; }
    }
}