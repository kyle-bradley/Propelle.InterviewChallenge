namespace Propelle.InterviewChallenge.Application
{
    public static class RetryFunctions
    {//I would add more configuration and methods to cater for different use-cases.
     //That said, rather use Polly as it already has these functions & multiple different strategies built-in.
        private static TimeSpan _defaultBaseDelay = TimeSpan.FromMilliseconds(50);
        private static int _baseExponentialRate = 2;
        private static TimeSpan _maxDelay = TimeSpan.FromHours(1);

        public static async Task ExecuteTask(Func<Task> action, uint retryAttempts = 6)
        {
            await RetryExecute(action, retryAttempts);
        }

        private static async Task RetryExecute(Func<Task> action, uint retryAttempts, int retryCount = 0)
        {
            if (retryCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(retryCount), $"Retry count provided {retryCount} was less than zero.");
            }

            try
            {
                await action();
            }
            catch (Exception)
            {
                var attemptsExceeded = retryCount >= retryAttempts;
                if (attemptsExceeded)
                {
                    throw;
                }

                var delayMultiplication = (_baseExponentialRate << retryCount) / 2;
                var delayCalculationWithCap = Math.Min(_defaultBaseDelay.TotalMilliseconds * delayMultiplication, _maxDelay.TotalMilliseconds);

                Console.Write(delayCalculationWithCap);
                await Task.Delay(TimeSpan.FromMilliseconds(delayCalculationWithCap));
                await RetryExecute(action, retryAttempts, ++retryCount);
            }
        }
    }
}
