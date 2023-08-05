using Newtonsoft.Json;

namespace HotelComparer.Models
{
    public class AmadeusApiTokenResponse
    {
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        [JsonProperty("username")]
        public string? UserName { get; set; }

        [JsonProperty("application_name")]
        public string? ApplicationName { get; set; }

        [JsonProperty("client_id")]
        public string? ClientId { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; } = string.Empty;

        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("state")]
        public string? State { get; set; }

        [JsonProperty("scope")]
        public string? Scope { get; set; }
    }
}
