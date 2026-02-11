using furkantural.Application.Models;
using furkantural.Application.Services.Abstract;
using Microsoft.Extensions.Options;

namespace furkantural.Infrastructure.Services.Concrete
{
    public class InMemoryEmailRateLimiter (
        IOptions<SmtpOptions> options
    ) : IEmailRateLimiter
    {
        private readonly object _lockObj = new();
        private readonly Queue<DateTime> _sendHistory = new();
        private readonly int _perHourLimit = options.Value.PerHourLimit;

        public async Task<bool> CanSendExternalEmail()
        {
            lock (_lockObj)
            {
                var now = DateTime.UtcNow;
                var oneHourAgo = now.AddHours(-1);

                while (_sendHistory.Count > 0 && _sendHistory.Peek() < oneHourAgo)
                {
                    _sendHistory.Dequeue();
                }

                if (_sendHistory.Count >= _perHourLimit)
                {
                    return false;
                }

                _sendHistory.Enqueue(now);
                return true;
            }
        }
    }
}