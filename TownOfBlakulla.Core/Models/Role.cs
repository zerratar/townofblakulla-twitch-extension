using System.Diagnostics;
using System.Linq;

namespace TownOfBlakulla.Core.Models
{
    public class Role
    {
        public Role(string name, string alignment, string summary, string abilities, string attribute)
        {
            Name = name;
            Alignment = alignment;
            Summary = summary;
            Abilities = abilities;
            Attribute = attribute;
        }

        public string Name { get; }
        public string Alignment { get; }
        public string Summary { get; }
        public string Abilities { get; }
        public string Attribute { get; }

        public string[] GetUsableArguments(PlayerInfo player, GameContext context)
        {
            if (Name == "Jester")
            {
                return GetJesterArguments(player, context);
            }

            if (player.Lynched)
            {
                return null;
            }

            switch (this.Name)
            {
                /*
                 * Townies
                 */
                case "Mayor": return GetMayorArguments(player, context);
                case "Jailor": return GetJailorArguments(player, context);
                case "Doctor": return GetDoctorArguments(player, context);
                case "Bodyguard": return GetBodyguardArguments(player, context);
                /*
                 * Mafia
                 */
                case "Mafioso": return GetMafiosoArguments(player, context);
                case "Godfather": return GetGodfatherArguments(player, context);
                case "Janitor": return GetJanitorArguments(player, context);
                case "Blackmailer": return GetBlackmailerArguments(player, context);
                /*
                 * Neutral
                 */
                case "SerialKiller": return GetSerialKillerArguments(player, context);
                case "Survivor": return GetSurvivorArguments(player, context);
            }

            return null;
        }

        private string[] GetSurvivorArguments(PlayerInfo player, GameContext context)
        {
            return null;
        }

        private string[] GetSerialKillerArguments(PlayerInfo player, GameContext context)
        {
            return null;
        }

        private string[] GetBlackmailerArguments(PlayerInfo player, GameContext context)
        {
            return null;
        }

        private string[] GetJanitorArguments(PlayerInfo player, GameContext context)
        {
            return null;
        }

        private string[] GetGodfatherArguments(PlayerInfo player, GameContext context)
        {
            return null;
        }

        private string[] GetMafiosoArguments(PlayerInfo player, GameContext context)
        {
            return null;
        }

        private string[] GetBodyguardArguments(PlayerInfo player, GameContext context)
        {
            return null;
        }

        private string[] GetDoctorArguments(PlayerInfo player, GameContext context)
        {
            if (context.GameState.PhaseName != "Night")
            {
                return null;
            }

            // TODO(Zerratar): Need to limit the use of healing themselves.

            return context.GameState.Players
                .Where(x => !x.Lynched)
                .Select(x => x.Name)
                .ToArray();
        }

        private string[] GetJailorArguments(PlayerInfo player, GameContext context)
        {
            if (context.GameState.PhaseName != "Day")
            {
                return null;
            }

            return context.GameState.Players
                .Where(x => x.Name != player.Name && !x.Lynched)
                .Select(x => x.Name)
                .ToArray();
        }

        private string[] GetJesterArguments(PlayerInfo player, GameContext context)
        {
            // TODO(Zerratar): We need to know who voted guilty and who abstained from voting
            return null;
        }

        private string[] GetMayorArguments(PlayerInfo player, GameContext context)
        {
            if (player.RevealedAsMayor || context.GameState.PhaseName != "Day")
            {
                return null;
            }

            // note(Zerratar): Leaving an empty string is still an "option", but is not targeting a player.
            return new[] { "" };
        }
    }
}