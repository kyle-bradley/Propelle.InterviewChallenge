using Propelle.InterviewChallenge.Application.Domain;
using Propelle.InterviewChallenge.Application.Domain.Events;
using Propelle.InterviewChallenge.EventHandling;

namespace Propelle.InterviewChallenge.Application.EventHandlers
{
    public class SubmitDeposit : IEventHandler<DepositMade>
    {
        private readonly ISmartInvestClient _smartInvestClient;

        public SubmitDeposit(ISmartInvestClient smartInvestClient)
        {
            _smartInvestClient = smartInvestClient;
        }

        public async Task Handle(DepositMade @event)
        {
            await _smartInvestClient.SubmitDeposit(@event.UserId, @event.Amount);
        }
    }
}
