using furkantural.Models;
using furkantural.Services.Abstract;
using Microsoft.Extensions.Options;
using System.Text.Json;


namespace furkantural.Services.Concrete
{
    public class TurnstileService (
        ILogService logService,
        HttpClient httpClient,
        IOptions<TurnstileViewModel> options
    ) : ITurnstileService
    {
        #region Fields
        private readonly ILogService _logService = logService;
        private readonly HttpClient _httpClient = httpClient;
        private readonly TurnstileViewModel _turnstileSettings = options.Value;
        #endregion

        #region Methods
        public async Task<bool> ValidateTokenAsync(string token, string? remoteIp = null, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                await _logService.Log(LogLevel.Error, "Turnstile token doğrulama başarısız: Token boş!");
                return false;
            }

            try
            {
                var form = new Dictionary<string, string>
                {
                    { "secret", _turnstileSettings.SecretKey },
                    { "response", token }
                };

                if (!string.IsNullOrEmpty(remoteIp))
                    form["remoteip"] = remoteIp;

                var content = new FormUrlEncodedContent(form);
                var response = await _httpClient.PostAsync($"{_turnstileSettings.BaseUrl}/turnstile/v0/siteverify", content, ct);

                if (!response.IsSuccessStatusCode)
                {
                    await _logService.Log(LogLevel.Error, $"Turnstile token doğrulama başarısız: {response.StatusCode}");
                    return false;
                }

                using var st = await response.Content.ReadAsStreamAsync(ct);
                var doc = await JsonSerializer.DeserializeAsync<TurnstileResponseViewModel>(st, cancellationToken: ct);

                if (doc == null || !doc.Success)
                {
                    await _logService.Log(LogLevel.Error, $"Turnstile token doğrulama başarısız: {(doc?.ErrorCodes != null ? string.Join(", ", doc.ErrorCodes) : "Bilinmeyen hata!")}");
                    return false;
                }

                await _logService.Log(LogLevel.Information, "Turnstile token doğrulama başarılı.");
                return true;
            }
            catch (Exception ex)
            {
                await _logService.Log(LogLevel.Error, $"Turnstile token doğrulama hatası: {ex.Message}");
                return false;
            }
        }
        #endregion
    }
}