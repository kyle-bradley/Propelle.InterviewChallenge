using Propelle.InterviewChallenge.Application.Domain;
using Propelle.InterviewChallenge.Application.Domain.Events;
using Propelle.InterviewChallenge.EventHandling;

namespace Propelle.InterviewChallenge.Application.EventHandlers
{
    public class PersistDeposit : IEventHandler<DepositMade>
    {
        private readonly PaymentsContext _paymentsContext;

        public PersistDeposit(PaymentsContext paymentsContext)
        {
            _paymentsContext = paymentsContext;
        }

        public async Task Handle(DepositMade @event)
        {
            var deposit = new Deposit(@event.Id, @event.UserId, @event.Amount);
            try
            {
                _paymentsContext.Deposits.Add(deposit);
                await _paymentsContext.SaveChangesAsync();

            }
            catch(Exception)
            {
                _paymentsContext.Deposits.Remove(deposit);
                throw;
            }
        }
    }
}
