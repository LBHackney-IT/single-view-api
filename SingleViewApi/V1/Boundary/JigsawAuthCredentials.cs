using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SingleViewApi.V1.Boundary
{
    public class CsrfTokenResponse
    {
        public string Token { get; set; }
        public List<string> Cookies { get; set; }
    }

    public class Credentials
    {
        public string HashedUsername { get; set; }
        public string HashedPassword { get; set; }
    }


}
