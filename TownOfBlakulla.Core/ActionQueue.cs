using System.Collections.Concurrent;
using TownOfBlakulla.Core.Handlers;
using TownOfBlakulla.Core.Models;

namespace TownOfBlakulla.Core
{
    public class ActionQueue : IActionQueue
    {
        private readonly IPropertyRepository propertyRepository;

        private readonly ConcurrentQueue<GameAction> queue;

        public ActionQueue(IPropertyRepository propertyRepository)
        {
            this.propertyRepository = propertyRepository;
            this.queue = this.propertyRepository.Load<ConcurrentQueue<GameAction>>(nameof(queue))
                ?? new ConcurrentQueue<GameAction>();
        }

        public bool TryDequeue(out GameAction action)
        {
            if (queue.TryDequeue(out action))
            {
                propertyRepository.Save(nameof(queue), queue);
                return true;
            }

            return false;
        }

        public void Enqueue(GameAction action)
        {
            queue.Enqueue(action);
            propertyRepository.Save(nameof(queue), queue);            
        }
    }
}