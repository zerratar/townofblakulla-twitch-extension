using System.Threading.Tasks;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core
{
    public interface IGame
    {
        void Update(UpdateGameStateRequest request);

        GameStateResponse GetState(TwitchViewer viewerContext);

        Task<LeaveResponse> LeaveAsync(TwitchViewer viewerContext);

        Task<JoinResponse> JoinAsync(TwitchViewer viewerContext, string name);

        Task<VoteResponse> VoteAsync(TwitchViewer viewerContext, string value);
    }
}