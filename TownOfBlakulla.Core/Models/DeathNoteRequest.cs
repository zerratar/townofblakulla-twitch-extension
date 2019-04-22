namespace TownOfBlakulla.Core.Models
{
    public class DeathNoteRequest
    {
        public DeathNoteRequest(string deathNote)
        {
            DeathNote = deathNote;
        }

        public string DeathNote { get; }
    }
}