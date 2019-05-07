using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core.Handlers
{
    public class PlayerHandler : IPlayerHandler
    {
        private readonly IPropertyRepository propertyRepository;

        private readonly object mutex = new object();

        private readonly List<TwitchViewer> viewers;

        private readonly ConcurrentDictionary<string, PlayerInfo> viewerPlayerLookup;

        private readonly ConcurrentDictionary<string, string> viewerPlayerNameLookup;

        public PlayerHandler(IPropertyRepository propertyRepository)
        {
            this.propertyRepository = propertyRepository;

            this.viewers =
                this.propertyRepository.Load<List<TwitchViewer>>(nameof(viewers))
                ?? new List<TwitchViewer>();

            this.viewerPlayerLookup =
                this.propertyRepository.Load<ConcurrentDictionary<string, PlayerInfo>>(nameof(viewerPlayerLookup))
                ?? new ConcurrentDictionary<string, PlayerInfo>();

            this.viewerPlayerNameLookup =
                this.propertyRepository.Load<ConcurrentDictionary<string, string>>(nameof(viewerPlayerNameLookup))
                ?? new ConcurrentDictionary<string, string>();
        }

        public void Reset()
        {
            lock (mutex)
            {
                viewers.Clear();
                viewerPlayerNameLookup.Clear();
                viewerPlayerLookup.Clear();
                SaveProperties();
            }
        }

        public void SetActivePlayers(IReadOnlyList<PlayerInfo> gameStatePlayers)
        {
            lock (mutex)
            {

                if (gameStatePlayers == null || gameStatePlayers.Count == 0)
                {
                    this.Reset();
                    return;
                }

                var changed = false;
                foreach (var viewer in this.viewers)
                {
                    if (viewerPlayerNameLookup.TryGetValue(viewer.Identifier, out var playerName))
                    {
                        viewerPlayerLookup[viewer.Identifier] =
                            gameStatePlayers.FirstOrDefault(x => x.Name == playerName);
                        changed = true;
                    }
                }

                if (changed)
                {
                    SaveProperties();
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
                SaveProperties();
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
                    SaveProperties();
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

        private void SaveProperties()
        {
            this.propertyRepository.Save(nameof(viewers), viewers);
            this.propertyRepository.Save(nameof(viewerPlayerLookup), viewerPlayerLookup);
            this.propertyRepository.Save(nameof(viewerPlayerNameLookup), viewerPlayerNameLookup);
        }
    }
}