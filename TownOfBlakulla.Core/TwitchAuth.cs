using System;
using System.Linq;
using TownOfBlakulla.Core.Models;
using TwitchLib.Extension;

namespace TownOfBlakulla.Core
{
    public class TwitchAuth : ITwitchAuth
    {
        private readonly ExtensionManager extensionManager;

        public TwitchAuth(ExtensionManager extensionManager)
        {
            this.extensionManager = extensionManager;
        }

        public bool Validate(string token, out TwitchViewer viewer)
        {
            viewer = null;

            if (string.IsNullOrEmpty(token))
                return false;

            if (token.StartsWith("bearer", StringComparison.InvariantCultureIgnoreCase))
                token = token.Substring(token.IndexOf(" ", StringComparison.Ordinal)).Trim();

            var extension = this.extensionManager.GetExtension("4bsfmhaxm72zd5izc8dj2ru7mqpmi0");
            var principal = extension.Verify(token, out var validTokenOverlay);

            var userId = principal.Claims.FirstOrDefault(x => x.Type == "user_id");
            var channelId = principal.Claims.FirstOrDefault(x => x.Type == "channel_id");
            var extensionId = principal.Claims.FirstOrDefault(x => x.Type == "extension_id");
            var opaqueUserId = principal.Claims.FirstOrDefault(x => x.Type == "opaque_user_id");

            //this.extensionManager.GetExtension()
            if ((opaqueUserId == null && userId == null) || channelId == null)
            {
                return false;
            }

            viewer = new TwitchViewer(
                   userId?.Value,
                   opaqueUserId?.Value,
                   channelId.Value,
                   extensionId?.Value);

            return true;
        }
    }
}