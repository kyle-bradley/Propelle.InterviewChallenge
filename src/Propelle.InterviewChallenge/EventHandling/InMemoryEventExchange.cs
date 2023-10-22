using Propelle.InterviewChallenge.Application;

namespace Propelle.InterviewChallenge.EventHandling
{
    public class InMemoryEventExchange
    {
        private readonly IServiceProvider _serviceProvider;

        public InMemoryEventExchange(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Publish<TEvent>(TEvent @event)
            where TEvent : class
        {
            using var scope = _serviceProvider.CreateScope();

            var eventHandlers = scope.ServiceProvider.GetServices(typeof(IEventHandler<TEvent>))
                .Cast<IEventHandler<TEvent>>();

            foreach (var eventHandler in eventHandlers)
            {
                await RetryFunctions.ExecuteTask(() => eventHandler.Handle(@event));
            }
        }
    }
}
