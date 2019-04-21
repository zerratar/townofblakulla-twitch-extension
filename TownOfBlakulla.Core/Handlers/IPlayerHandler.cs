using System.Collections.Generic;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core.Handlers
{
    public interface IPlayerHandler
    {
        void Reset();

        void SetActivePlayers(IReadOnlyList<PlayerInfo> gameStatePlayers);
        PlayerInfo Get(TwitchViewer viewerContext);

        void Add(TwitchViewer viewer, string playerName);
        void Remove(TwitchViewer viewer);
        bool IsPlaying(TwitchViewer viewer);
        string GetPlayerName(TwitchViewer viewer);
        
    }
}