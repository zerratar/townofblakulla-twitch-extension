using System.Collections.Generic;
using System.Text;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core
{
    public interface ITwitchAuth
    {
        bool Validate(string token, out TwitchViewer viewer);
    }
}
