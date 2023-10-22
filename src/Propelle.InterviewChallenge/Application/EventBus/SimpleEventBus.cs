using Propelle.InterviewChallenge.EventHandling;

namespace Propelle.InterviewChallenge.Application.EventBus
{
    public class SimpleEventBus : IEventBus
    {
        private readonly InMemoryEventExchange _exchange;
        private readonly FileDeadLetterEventExchange _deadQueueExchange;

        public SimpleEventBus(InMemoryEventExchange exchange, FileDeadLetterEventExchange deadQueueExchange)
        {
            _exchange = exchange;
            _deadQueueExchange = deadQueueExchange;
        }

        public async Task Publish<TEvent>(TEvent @event)
            where TEvent : class
        {
            /* If you've found this, you're eagled-eyed! Let us know in the interview if you see this, and have a think about the ramifications of 
             * changing SimulatePotentialFailure() to have a higher than zero chance of throwing an exception (i.e. simulating a real event-bus being unavailable at times) */

            try
            {
                PointOfFailure.SimulatePotentialFailure(0);

                await _exchange.Publish(@event);
            }
            catch (Exception)
            {
                await _deadQueueExchange.Publish(@event);
            }
        }
    }
}
