using furkantural.Application.Models;
using furkantural.Application.Services.Abstract;
using Microsoft.Extensions.Options;
using System.Text.Json;
using furkantural.Application.Wrappers;

namespace furkantural.Infrastructure.Services.Concrete
{
    public class TurnstileService (
        ILogService logService,
        HttpClient httpClient,
        IOptions<TurnstileOptions> options
    ) : ITurnstileService
    {
        private readonly ILogService _logService = logService;
        private readonly HttpClient _httpClient = httpClient;
        private readonly TurnstileOptions _turnstileSettings = options.Value;

        public async Task<Result> ValidateTokenAsync(string token, string? remoteIp = null, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                await _logService.Error("Turnstile token doğrulama başarısız: Token boş!");
                return Result.Fail("Token is empty.");
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
                    await _logService.Error($"Turnstile token doğrulama başarısız: {response.StatusCode}");
                    return Result.Fail($"Verification check failed: {response.StatusCode}");
                }

                using var st = await response.Content.ReadAsStreamAsync(ct);
                var doc = await JsonSerializer.DeserializeAsync<TurnstileApiResponse>(st, cancellationToken: ct);

                if (doc == null || !doc.Success)
                {
                    var errorMsg = doc?.ErrorCodes != null ? string.Join(", ", doc.ErrorCodes) : "Unknown error";
                    await _logService.Error($"Turnstile token doğrulama başarısız: {errorMsg}");
                    return Result.Fail($"Security check failed: {errorMsg}");
                }

                await _logService.Info("Turnstile token doğrulama başarılı.");
                return Result.Ok("Verification successful.");
            }
            catch (Exception ex)
            {
                await _logService.Error($"Turnstile token doğrulama hatası: {ex.Message}");
                return Result.Fail($"Internal error: {ex.Message}");
            }
        }
    }
}