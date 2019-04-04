using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TownOfBlakulla.Core;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.EBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IGame game;
        private readonly IActionQueue actionQueue;

        public AdminController(IGame game, IActionQueue actionQueue)
        {
            this.game = game;
            this.actionQueue = actionQueue;
        }

        [HttpPost("state")]
        public void UpdateGameState(UpdateGameStateRequest request)
        {
            this.game.Update(request);
        }

        [HttpGet("poll")]
        public async Task<GameAction> PollAsync()
        {
            var timeout = 0;
            GameAction item = null;

            while (!actionQueue.TryDequeue(out item))
            {
                await Task.Delay(200);
                timeout += 200;
                if (timeout >= 20_000) return GameAction.None;
            }

            return item;
        }
    }
}