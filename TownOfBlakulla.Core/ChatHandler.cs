using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TownOfBlakulla.Core.Handlers;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core
{
    public class ChatHandler : IChatHandler
    {
        private readonly IPlayerHandler playerHandler;
        private readonly List<ChatMessage> chatMessages = new List<ChatMessage>();
        private readonly object mutex = new object();

        public ChatHandler(IPlayerHandler playerHandler)
        {
            this.playerHandler = playerHandler;
        }

        public async Task<IReadOnlyList<ChatMessage>> GetChatMessagesAsync(
            TwitchViewer viewerContext,
            string channel,
            DateTime since)
        {
            var elapsed = 0;
            var messages = GetChatMessages(viewerContext, channel, since);
            while (messages.Count == 0)
            {                
                elapsed += 50;
                await Task.Delay(50);
                messages = GetChatMessages(viewerContext, channel, since);
                if (elapsed >= 10000) break;
            }

            return messages;
        }

        public IReadOnlyList<ChatMessage> GetChatMessages(TwitchViewer viewerContext, string channel, DateTime since)
        {
            var player = this.playerHandler.Get(viewerContext);
            if (player == null)
            {
                return new ChatMessage[0];
            }

            if (!this.HasAccessToChatChannel(player, channel))
            {
                return new ChatMessage[0];
            }

            lock (mutex)
            {
                var messages = this.chatMessages
                    .Where(x => x.Channel == channel && x.Sender != player.Name && x.TimeSent > since)
                    .ToList();
                return messages;
            }
        }

        public ChatMessage HandleChatResponse(ChatResponse chatResponse)
        {
            lock (mutex)
            {
                var chatMessage = new ChatMessage(chatResponse.Username, chatResponse.Channel, chatResponse.Message);
                chatMessages.Add(chatMessage);
                return chatMessage;
            }
        }

        public void Reset()
        {
            lock (mutex)
            {
                chatMessages.Clear();
            }
        }

        private bool HasAccessToChatChannel(PlayerInfo player, string channel)
        {
            var playerRole = player.Role;

            var isMafia = IsMafia(playerRole.Alignment ?? playerRole.Name);
            var isMafiaChannel = IsMafiaChannel(channel);
            if (isMafiaChannel && isMafia)
            {
                return true;
            }

            var isGraveyardChannel = IsGraveyardChannel(channel);
            var isDead = player.Lynched;
            var isMedium = player.Role.Name.Equals("medium", StringComparison.OrdinalIgnoreCase);
            if (isGraveyardChannel && (isDead || isMedium))
            {
                return true;
            }

            var isTownChannel = IsTownChannel(channel);
            if (isTownChannel)
            {
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsTownChannel(string channel)
        {
            return channel.Equals("all", StringComparison.OrdinalIgnoreCase) ||
                   channel.Equals("town", StringComparison.OrdinalIgnoreCase) ||
                   channel.Equals("everyone", StringComparison.OrdinalIgnoreCase);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsGraveyardChannel(string channel)
        {
            return channel.Equals("graveyard", StringComparison.OrdinalIgnoreCase) ||
                   channel.Equals("dead", StringComparison.OrdinalIgnoreCase) ||
                   channel.Equals("medium", StringComparison.OrdinalIgnoreCase);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsMafiaChannel(string channelName)
        {
            return channelName.Equals("mafia", StringComparison.OrdinalIgnoreCase);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsMafia(string roleName)
        {
            return roleName.Equals("mafia", StringComparison.OrdinalIgnoreCase) || Array.IndexOf(mafiaRoles, roleName) >= 0;
        }

        private static readonly string[] mafiaRoles = new string[]
        {
            "Janitor", "Godfather", "Blackmailer", "Mafioso",
            "Ambusher", "Consigliere", "Consort", "Hypnotist",
            "Framer", "Forger", "Disguiser"
        };
    }
}