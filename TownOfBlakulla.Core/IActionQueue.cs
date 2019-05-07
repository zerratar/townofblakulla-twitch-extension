using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core
{
    public interface IActionQueue
    {
        void Enqueue(GameAction action);
        bool TryDequeue(out GameAction action);
    }
}