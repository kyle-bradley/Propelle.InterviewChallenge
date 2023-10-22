using FastEndpoints;
using Propelle.InterviewChallenge.EventHandling;

namespace Propelle.InterviewChallenge.Endpoints
{
    public static class ProcessEventBacklog
    {
        private const string _eventNamespace = "Propelle.InterviewChallenge.Application.Domain.Events";

        public class Request
        {
            public string EventType { get; set; }
        }

        public class Response
        {   }

        public class Endpoint : Endpoint<Request, Response>
        {
            private readonly FileDeadLetterEventExchange _deadQueue;
            private readonly Application.EventBus.IEventBus _eventBus;

            public Endpoint(FileDeadLetterEventExchange deadQueue, Application.EventBus.IEventBus eventBus)
            {
                _deadQueue = deadQueue;
                _eventBus = eventBus;
            }

            public override void Configure()
            {
                Post("/api/backlogs");
            }

            public override async Task HandleAsync(Request req, CancellationToken ct)
            {
                var eventType = Type.GetType($"{_eventNamespace}.{req.EventType}");

                var methodInfo = typeof(FileDeadLetterEventExchange).GetMethod(nameof(_deadQueue.ConsumeAndPublish));
                var genericMethod = methodInfo.MakeGenericMethod(new[] { eventType });
                await ((Task) genericMethod.Invoke(_deadQueue, null));
                
                await SendAsync(new Response { }, 201, ct);
            }
        }
    }
}
