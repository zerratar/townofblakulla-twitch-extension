using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core
{
    public interface IGame
    {
        void Update(UpdateGameStateRequest request);

        void PushMessages(MessageRequest request);

        GameStateResponse GetState(TwitchViewer viewerContext);

        Task<GameStateResponse> GetStateAsync(TwitchViewer viewerContext, int revision);

        Task<LeaveResponse> LeaveAsync(TwitchViewer viewerContext);

        Task<JoinResponse> JoinAsync(TwitchViewer viewerContext, string name);

        Task<VoteResponse> VoteAsync(TwitchViewer viewerContext, string value);

        Task<ChatMessage> SendChatMessageAsync(TwitchViewer viewerContext, string channel, string message);

        Task<LastWillResponse> UpdateLastWill(TwitchViewer viewerContext, string lastWill);

        Task<DeathNoteResponse> UpdateDeathNote(TwitchViewer viewerContext, string deathNote);
    }
}