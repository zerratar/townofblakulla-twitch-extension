namespace TownOfBlakulla.Core.Models
{
    public class TwitchViewer
    {
        public TwitchViewer(string userId, string opaqueUserId, string channelId, string extensionId)
        {
            UserId = userId;
            OpaqueUserId = opaqueUserId;
            ChannelId = channelId;
            ExtensionId = extensionId;
        }

        public string UserId { get; }
        public string OpaqueUserId { get; }
        public string ChannelId { get; }
        public string ExtensionId { get; }


        public string Identifier => UserId ?? OpaqueUserId;

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }
    }
}