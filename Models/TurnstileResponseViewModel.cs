using System.Text.Json.Serialization;

namespace furkantural.Models
{
    public class TurnstileResponseViewModel
    {
        #region Properties
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error-codes")]
        public string[]? ErrorCodes { get; set; }

        [JsonPropertyName("challenge_ts")]
        public string? ChallengeTs { get; set; }

        [JsonPropertyName("hostname")]
        public string? Hostname { get; set; }
        #endregion
    }
}