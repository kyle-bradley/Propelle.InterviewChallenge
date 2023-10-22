namespace Propelle.InterviewChallenge.Application.Domain.Events
{
    public class DepositMade
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
    }
}
