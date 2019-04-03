using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TwitchLib.Extension;

namespace TownOfBlakulla.EBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = "TwitchExtensionAuth", Policy = "TownofBlakullaAuth")]
    public class BlakullaController : ControllerBase
    {
        private readonly ExtensionManager extensionManager;

        public BlakullaController(ExtensionManager extensionManager)
        {
            this.extensionManager = extensionManager;
        }
        [HttpPost("test")]
        public string Test1()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return "NOT OK";
            }

            //string jwt = Request.Headers["x-extension-jwt"];
            string auth = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(auth)) return "NOT OK";

            if (auth.StartsWith("bearer", StringComparison.InvariantCultureIgnoreCase))
                auth = auth.Substring(auth.IndexOf(" ")).Trim();

            var extension = this.extensionManager.GetExtension("4bsfmhaxm72zd5izc8dj2ru7mqpmi0");
            var principal = extension.Verify(auth, out var validTokenOverlay);



            var userId = principal.Claims.FirstOrDefault(x => x.Type == "user_id");
            var channelId = principal.Claims.FirstOrDefault(x => x.Type == "channel_id");
            var extensionId = principal.Claims.FirstOrDefault(x => x.Type == "extension_id");

            //this.extensionManager.GetExtension()

            if (userId == null || channelId == null)
            {
                return "NOT OK";
            }

            return $"OK! {userId.Value}";
        }

        [HttpGet("test")]
        public string Test()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return "NOT OK";
            }

            //string jwt = Request.Headers["x-extension-jwt"];
            string auth = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(auth)) return "NOT OK";

            if (auth.StartsWith("bearer", StringComparison.InvariantCultureIgnoreCase))
                auth = auth.Substring(auth.IndexOf(" ")).Trim();

            var extension = this.extensionManager.GetExtension("4bsfmhaxm72zd5izc8dj2ru7mqpmi0");
            var principal = extension.Verify(auth, out var validTokenOverlay);



            var userId = principal.Claims.FirstOrDefault(x => x.Type == "user_id");
            var channelId = principal.Claims.FirstOrDefault(x => x.Type == "channel_id");
            var extensionId = principal.Claims.FirstOrDefault(x => x.Type == "extension_id");

            //this.extensionManager.GetExtension()

            if (userId == null || channelId == null)
            {
                return "NOT OK";
            }

            return $"OK! {userId.Value}";
        }

    }
}