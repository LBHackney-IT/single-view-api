using System.Collections.Generic;

namespace SingleViewApi.V1.Boundary
{
    public class JigsawAuthCredentials
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string RequestVerificationToken { get; set; }
    }

    public class CsrfTokenResponse
    {
        public string Token { get; set; }
        public List<string> Cookies { get; set; }
    }


}
