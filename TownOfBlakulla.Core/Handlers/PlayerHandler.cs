using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core.Handlers
{
    public class PlayerHandler : IPlayerHandler
    {
        private readonly object mutex = new object();

        private readonly List<TwitchViewer> viewers = new List<TwitchViewer>();

        private readonly ConcurrentDictionary<string, PlayerInfo> viewerPlayerLookup
            = new ConcurrentDictionary<string, PlayerInfo>();

        private readonly ConcurrentDictionary<string, string> viewerPlayerNameLookup
            = new ConcurrentDictionary<string, string>();

        public void Reset()
        {
            lock (mutex)
            {
                viewers.Clear();
                viewerPlayerNameLookup.Clear();
                viewerPlayerLookup.Clear();
            }
        }

        public void SetActivePlayers(IReadOnlyList<PlayerInfo> gameStatePlayers)
        {
            lock (mutex)
            {
                foreach (var viewer in this.viewers)
                {
                    if (viewerPlayerNameLookup.TryGetValue(viewer.Identifier, out var playerName))
                    {
                        viewerPlayerLookup[viewer.Identifier] =
                            gameStatePlayers.FirstOrDefault(x => x.Name == playerName);
                    }

                }
            }
        }

        public PlayerInfo Get(TwitchViewer viewerContext)
        {
            if (this.viewerPlayerLookup.TryGetValue(viewerContext.Identifier, out var player))
                return player;

            return null;
        }

        public void Add(TwitchViewer viewer, string playerName)
        {
            lock (mutex)
            {
                viewers.Add(viewer);
                viewerPlayerNameLookup[viewer.Identifier] = playerName;
            }
        }

        public void Remove(TwitchViewer viewer)
        {
            lock (mutex)
            {
                var toRemove = viewers.FirstOrDefault(x => x.Identifier == viewer.Identifier);
                if (toRemove != null)
                {
                    viewers.Remove(toRemove);
                    viewerPlayerNameLookup.Remove(viewer.Identifier, out _);
                }
            }
        }

        public bool IsPlaying(TwitchViewer viewer)
        {
            lock (mutex)
            {
                return viewers.Any(x => x.Identifier == viewer.Identifier);
            }
        }

        public string GetPlayerName(TwitchViewer viewer)
        {
            if (this.viewerPlayerNameLookup.TryGetValue(viewer.Identifier, out var name))
            {
                return name;
            }

            return null;
        }
    }
}