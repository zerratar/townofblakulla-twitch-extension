using System.Collections.Concurrent;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core
{
    public interface IActionQueue
    {
        void Enqueue(GameAction action);
        bool TryDequeue(out GameAction action);
    }

    public class ActionQueue : IActionQueue
    {
        private readonly ConcurrentQueue<GameAction> queue = new ConcurrentQueue<GameAction>();

        public bool TryDequeue(out GameAction action)
        {
            return queue.TryDequeue(out action);
        }

        public void Enqueue(GameAction action)
        {
            queue.Enqueue(action);
        }
    }
}