using FastEndpoints;
using Propelle.InterviewChallenge.Application;
using Propelle.InterviewChallenge.Application.Domain.Events;
using System.Reflection;

namespace Propelle.InterviewChallenge.Endpoints
{
    public static class MakeDeposit
    {
        public class Request
        {
            public Guid UserId { get; set; }

            public decimal Amount { get; set; }
        }

        public class Response
        {
            public Guid DepositId { get; set; }
        }

        public class Endpoint : Endpoint<Request, Response>
        {
            private readonly Application.EventBus.IEventBus _eventBus;

            public Endpoint(Application.EventBus.IEventBus eventBus)
            {
                _eventBus = eventBus;
            }

            public override void Configure()
            {
                Post("/api/deposits/{UserId}");
            }

            public override async Task HandleAsync(Request req, CancellationToken ct)
            {//I would create a domain layer that is separated from the database logic to do some validation before publishing the event.
                var depositEvent = new DepositMade
                {
                    Id = Guid.NewGuid(),
                    UserId = req.UserId,
                    Amount = req.Amount
                };

                await _eventBus.Publish(depositEvent);

                await SendAsync(new Response { DepositId = depositEvent.Id }, 201, ct);
            }
        }
    }
}
