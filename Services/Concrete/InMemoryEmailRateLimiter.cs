using furkantural.Services.Abstract;

namespace furkantural.Services.Concrete
{
    public class InMemoryEmailRateLimiter (
        IConfiguration configuration
    ) : IEmailRateLimiter
    {
        #region Fields
        private readonly object _lockObj = new();
        private readonly Queue<DateTime> _sendHistory = new();
        private readonly int _perHourLimit = int.TryParse(configuration["DoNotReplyEmailSettings:PerHourLimit"], out var val) ? val : 50;
        #endregion

        #region Methods

        #endregion
        public async Task<bool> CanSendExternalEmail()
        {
            lock (_lockObj)
            {
                var now = DateTime.UtcNow;
                var oneHourAgo = now.AddHours(-1);

                // Eski kayıtları sil
                while (_sendHistory.Count > 0 && _sendHistory.Peek() < oneHourAgo)
                {
                    _sendHistory.Dequeue();
                }

                if (_sendHistory.Count >= _perHourLimit)
                {
                    // limit dolu
                    return false;
                }

                // limit içinde -> bu maili rezerve et
                _sendHistory.Enqueue(now);
                return true;
            }
        }
    }
}
