// File: Models/AmadeusApiTokenResponse.cs

namespace HotelComparer.Models
{
    public class AmadeusApiTokenResponse
    {
        public string Type { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? ApplicationName { get; set; }
        public string? ClientId { get; set; }
        public string TokenType { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string? State { get; set; }
        public string? Scope { get; set; }
    }
}
