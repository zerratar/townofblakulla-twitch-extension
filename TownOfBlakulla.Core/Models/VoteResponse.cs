namespace TownOfBlakulla.Core.Models
{
    public class VoteResponse
    {
        public VoteResponse(string message)
        {
            this.Message = message;
        }

        public string Message { get; }
    }
}