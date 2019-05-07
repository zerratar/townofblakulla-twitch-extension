namespace TownOfBlakulla.Core.Models
{
    public class UseAbilityResponse
    {
        public UseAbilityResponse(string errorMessage = null)
        {
            this.ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }

        public bool Success => string.IsNullOrEmpty(ErrorMessage);
    }
}