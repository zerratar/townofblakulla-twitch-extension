using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core
{
    public interface IChatHandler
    {
        IReadOnlyList<ChatMessage> GetChatMessages(TwitchViewer viewerContext, string channel, DateTime since);
        Task<IReadOnlyList<ChatMessage>> GetChatMessagesAsync(TwitchViewer viewerContext, string channel, DateTime since);
        ChatMessage HandleChatResponse(ChatResponse chatResponse);
        void Reset();
    }
}