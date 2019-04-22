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

        [HttpPost("messages")]
        public void PushMessages(MessageRequest request)
        {
            this.game.PushMessages(request);
        }

        [HttpGet("poll")]
        public async Task<GameAction> PollAsync()
        {
            var timeout = 0;
            GameAction item = null;

            while (!actionQueue.TryDequeue(out item))
            {
                await Task.Delay(10);
                timeout += 10;
                if (timeout >= 10000) return GameAction.None;
            }

            return item;
        }
    }
}