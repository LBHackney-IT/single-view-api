using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SingleViewApi.V1.Boundary
{
    public class CsrfTokenResponse
    {
        public string Token { get; set; }
        public List<string> Cookies { get; set; }
    }

    public class AuthGatewayResponse
    {
        public string Token { get; set; }
#nullable enable
        public string? ExceptionMessage { get; set; }
#nullable disable
    }


}
