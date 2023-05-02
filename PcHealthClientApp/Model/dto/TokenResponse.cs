using System.Text.Json.Serialization;

namespace PcHealthClientApp.Model.dto
{
    public class TokenResponse
    {
		[JsonPropertyName("accesToken")]
		public string AccesToken { get; set; }

		[JsonPropertyName("refreshToken")]
		public string RefreshToken { get; set; }
    }
}
