using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SingleViewApi.V1.Boundary
{
    public class JigsawAuthCredentials
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [JsonPropertyName("__RequestVerificationToken")]
        public string RequestVerificationToken { get; set; }
    }

    public class CsrfTokenResponse
    {
        public string Token { get; set; }
        public List<string> Cookies { get; set; }
    }


}
