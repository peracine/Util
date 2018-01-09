using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Util.Retry
{
    public class Retry
    {
        private readonly ILogger _logger;
        private readonly TimeSpan Default_Delay = TimeSpan.FromMilliseconds(500);
        private readonly int Maximum_Delay = 30000;

        public Retry(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException("'logger' parameter cannot be null.");
        }

        /// <summary>
        /// Calls [maxAttempts] an async function [operation]
        /// Implement the try pattern
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="operation">Not null</param>
        /// <param name="backoffType">Fixed default</param>
        /// <param name="maxAttempts">5 default</param>
        /// <param name="delay">In milliseconds, 500 default</param>
        /// <returns></returns>
        public async Task RetryOnExceptionAsync<TException>(Func<Task> operation, BackoffType backoffType = BackoffType.Fixed, uint maxAttempts = 5, TimeSpan? delay = null)
            where TException : Exception
        {
            if (operation == null)
                return;

            if (delay == null)
                delay = Default_Delay;

            for (int attempt = 0; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    await operation();
                    break;
                }
                catch (TException exception)
                {
                    if (attempt == maxAttempts)
                    {
                        _logger.LogError(exception, $"Could not execute the operation after {maxAttempts} attempts.");
                        throw;
                    }

                    _logger.LogWarning(exception, $"Exception {exception.GetType().Name} on attempt {attempt + 1} of {maxAttempts}.");

                    await Task.Delay(GetDelay(backoffType, attempt, delay.Value));
                }
            }
        }

        private int GetDelay(BackoffType backoffType, int attempt, TimeSpan delay)
        {
            switch (backoffType)
            {
                case BackoffType.Linear:
                    return Convert.ToInt32(attempt * delay.TotalMilliseconds);

                case BackoffType.Exponential: // IEEE Standard 802.3-2008
                    return Convert.ToInt32(delay.TotalMilliseconds * (Math.Pow(2, Convert.ToDouble(attempt)) - 1) / 2);

                case BackoffType.Random:
                    var random = new Random();
                    return random.Next(1000, Maximum_Delay);

                default: // BackoffType.Fixed by default
                    return Convert.ToInt32(delay.TotalMilliseconds);
            }
        }
    }
}
