namespace TownOfBlakulla.Core.Models
{
    public class VoteRequest
    {

        public VoteRequest(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}