using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SingleViewApi.V1.Boundary
{
    public class CsrfTokenResponse
    {
        public string Token { get; set; }
        public List<string> Cookies { get; set; }
    }


}
