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

        private bool TryGetViewer(out TwitchViewer user)
        {
            user = null;
            return
                Request.Headers.ContainsKey("Authorization") &&
                auth.Validate(Request.Headers["Authorization"], out user);
        }
    }
}