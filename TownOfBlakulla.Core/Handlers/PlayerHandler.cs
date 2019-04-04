using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core.Handlers
{
    public class PlayerHandler : IPlayerHandler
    {
        private readonly object mutex = new object();
        private readonly List<TwitchViewer> viewers;

        public PlayerHandler()
        {
            viewers = new List<TwitchViewer>();
        }

        public void Reset()
        {
            lock (mutex)
            {
                viewers.Clear();
            }
        }

        public void Add(TwitchViewer viewer)
        {
            lock (mutex)
            {
                viewers.Add(viewer);
            }
        }

        public void Remove(TwitchViewer viewer)
        {
            lock (mutex)
            {
                viewers.Remove(viewer);
            }
        }

        public bool IsPlaying(TwitchViewer viewer)
        {
            lock (mutex)
            {
                return viewers.Any(x => x.Identifier == viewer.Identifier);
            }
        }
    }
}