using System.Diagnostics;
using System.Linq;

namespace TownOfBlakulla.Core.Models
{
    public class Role
    {
        private const string Night = "Night";
        private const string Day = "Day";

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
                return new string[0];
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

            return new string[0];
        }

        private string[] GetSurvivorArguments(PlayerInfo player, GameContext context)
        {
            if (context.GameState.PhaseName != Day)
            {
                return new string[0];
            }

            return new string[] { "" };
        }

        private string[] GetSerialKillerArguments(PlayerInfo player, GameContext context)
        {
            return AnyoneDuringNight(player, context);
        }

        private string[] GetBlackmailerArguments(PlayerInfo player, GameContext context)
        {
            return GetMafiosoArguments(player, context);
        }

        private string[] GetJanitorArguments(PlayerInfo player, GameContext context)
        {
            return AnyoneDuringNight(player, context);
        }

        private string[] GetGodfatherArguments(PlayerInfo player, GameContext context)
        {
            return GetMafiosoArguments(player, context);
        }

        private string[] GetMafiosoArguments(PlayerInfo player, GameContext context)
        {
            if (context.GameState.PhaseName != Night)
            {
                return new string[0];
            }

            // crashes here for some reason
            // x.Role == null?
            // gameState players == null?

            // TODO(Zerratar): fixme! Not all players have an assigned role. Why? We need to debug this!

            return context.GameState.Players
                .Where(x => !x.Lynched && x != player && x.Role.Alignment != Alignment)
                .Select(x => x.Name)
                .ToArray();
        }

        private string[] GetBodyguardArguments(PlayerInfo player, GameContext context)
        {
            return AnyoneDuringNight(player, context);
        }

        private string[] GetDoctorArguments(PlayerInfo player, GameContext context)
        {
            if (context.GameState.PhaseName != Night)
            {
                return new string[0];
            }

            // TODO(Zerratar): Need to limit the use of healing themselves.
            return context.GameState.Players
                .Where(x => !x.Lynched)
                .Select(x => x.Name)
                .ToArray();
        }

        private string[] GetJailorArguments(PlayerInfo player, GameContext context)
        {
            return AnyoneDuringDay(player, context);
        }

        private string[] GetJesterArguments(PlayerInfo player, GameContext context)
        {
            // TODO(Zerratar): We need to know who voted guilty and who abstained from voting
            return new string[0];
        }

        private string[] GetMayorArguments(PlayerInfo player, GameContext context)
        {
            if (player.RevealedAsMayor || context.GameState.PhaseName != Day)
            {
                return new string[0];
            }

            // note(Zerratar): Leaving an empty string is still an "option", but is not targeting a player.
            return new[] { "" };
        }

        private static string[] AnyoneDuringDay(PlayerInfo player, GameContext context)
        {
            return context.GameState.PhaseName == Day ? Anyone(player, context) : null;
        }

        private static string[] AnyoneDuringNight(PlayerInfo player, GameContext context)
        {
            return context.GameState.PhaseName == Night ? Anyone(player, context) : null;
        }

        private static string[] Anyone(PlayerInfo player, GameContext context)
        {
            return context.GameState.Players
                .Where(x => !x.Lynched && x != player)
                .Select(x => x.Name)
                .ToArray();
        }
    }
}