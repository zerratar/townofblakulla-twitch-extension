using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TownOfBlakulla.Core;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.EBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlakullaController : ControllerBase
    {
        private readonly ITwitchAuth auth;
        private readonly IGame game;

        public BlakullaController(ITwitchAuth auth, IGame game)
        {
            this.auth = auth;
            this.game = game;
        }

        [HttpGet("state")]
        public GameStateResponse GetGameState()
        {
            if (!TryGetViewer(out var user))
                return null;

            return this.game.GetState(user);
        }

        [HttpPost("leave")]
        public Task<LeaveResponse> LeaveGame()
        {
            if (!TryGetViewer(out var user))
                return null;

            return this.game.LeaveAsync(user);
        }

        [HttpPost("join")]
        public Task<JoinResponse> JoinGame(string name)
        {
            if (!TryGetViewer(out var user))
                return null;

            return this.game.JoinAsync(user, name);
        }

        [HttpPost("vote")]
        public Task<VoteResponse> Vote(string value)
        {
            if (!TryGetViewer(out var user))
                return null;

            return this.game.VoteAsync(user, value);
        }

        private bool TryGetViewer(out TwitchViewer user)
        {
            user = null;
            return
                Request.Headers.ContainsKey("Authorization") &&
                auth.Validate(Request.Headers["Authorization"], out user);
        }
    }
}