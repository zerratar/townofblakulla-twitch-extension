namespace TownOfBlakulla.Core.Models
{
    public class LastWillRequest
    {
        public LastWillRequest(string lastWill)
        {
            LastWill = lastWill;
        }

        public string LastWill { get; }
    }
}