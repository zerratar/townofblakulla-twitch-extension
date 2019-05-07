using System;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core
{
    public class GameContext
    {
        private int revision;
        private GameUpdateInfo gameState;

        private GameContext()
        {
        }

        public GameContext(GameUpdateInfo gameState, int revision)
        {
            this.gameState = gameState;
            this.revision = revision;
        }

        public GameUpdateInfo GameState
        {
            get => gameState;
            set
            {
                if (RequiresRevisionUpdate(gameState, value))
                {
                    ++revision;
                }

                gameState = value;
            }
        }

        public int Revision => revision;

        private bool RequiresRevisionUpdate(GameUpdateInfo oldState, GameUpdateInfo newState)
        {
            if (oldState == null && newState == null)
            {
                return false;
            }

            if (oldState == null)
            {
                return true;
            }

            if (newState == null)
            {
                return true;
            }

            if (oldState.DaysPassed != newState.DaysPassed)
            {
                return true;
            }

            if (oldState.Started != newState.Started)
            {
                return true;
            }

            if (oldState.Joinable != newState.Joinable)
            {
                return true;
            }

            if (oldState.PhaseName != newState.PhaseName)
            {
                return true;
            }

            if (oldState.SubPhase == null)
            {
                return true;
            }

            if (oldState.SubPhase.Name != newState.SubPhase.Name)
            {
                return true;
            }

            for (var i = 0; i < oldState.Players.Count; ++i)
            {
                var oldPlayer = oldState.Players[i];
                var newPlayer = newState.Players[i];
                if (oldPlayer.Role?.Name != newPlayer.Role?.Name ||
                    oldPlayer.Lynched != newPlayer.Lynched)
                {
                    return true;
                }
            }

            return false;
        }

        public static GameContext CreateEmpty()
        {
            return new GameContext();
        }
    }
}