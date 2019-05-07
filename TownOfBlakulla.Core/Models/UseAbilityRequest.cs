namespace TownOfBlakulla.Core.Models
{
    public class UseAbilityRequest
    {
        public UseAbilityRequest(string arguments)
        {
            Arguments = arguments;
        }

        public string Arguments { get; }
    }
}