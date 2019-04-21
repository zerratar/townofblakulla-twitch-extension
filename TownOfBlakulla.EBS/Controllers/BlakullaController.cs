using System;
using System.Collections.Generic;
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
        private readonly IGame game;
        private readonly ITwitchAuth auth;
        private readonly IChatHandler chatHandler;

        public BlakullaController(
            IGame game,
            ITwitchAuth auth,
            IChatHandler chatHandler)
        {
            this.auth = auth;
            this.game = game;
            this.chatHandler = chatHandler;
        }

        [HttpGet("state/{revision}")]
        public Task<GameStateResponse> GetGameState(int revision)
        {
            if (!TryGetViewer(out var user))
                return null;

            return this.game.GetStateAsync(user, revision);
        }

        [HttpGet("chat/{channel}/{since}")]
        public Task<IReadOnlyList<ChatMessage>> GetChatMessages(string channel, DateTime since)
        {
            if (!TryGetViewer(out var user))
                return null;

            return this.chatHandler.GetChatMessagesAsync(user, channel, since.ToUniversalTime());
        }

        [HttpPost("leave")]
        public Task<LeaveResponse> LeaveGame()
        {
            if (!TryGetViewer(out var user))
                return null;

            return this.game.LeaveAsync(user);
        }

        [HttpPost("join")]
        public Task<JoinResponse> JoinGame(JoinGameRequest request)
        {
            if (!TryGetViewer(out var user))
                return null;

            return this.game.JoinAsync(user, request.Name);
        }

        [HttpPost("vote")]
        public Task<VoteResponse> Vote(VoteRequest request)
        {
            if (!TryGetViewer(out var user))
                return null;

            return this.game.VoteAsync(user, request.Value);
        }

        [HttpPost("chat")]
        public Task<ChatMessage> Chat(ChatRequest request)
        {
            if (!TryGetViewer(out var user))
                return null;

            return this.game.SendChatMessageAsync(user, request.Channel, request.Message);
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