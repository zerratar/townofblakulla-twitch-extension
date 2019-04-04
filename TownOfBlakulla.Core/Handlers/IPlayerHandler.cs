using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core.Handlers
{
    public interface IPlayerHandler
    {
        void Reset();
        void Add(TwitchViewer viewer);
        void Remove(TwitchViewer viewer);
        bool IsPlaying(TwitchViewer viewer);
    }
}