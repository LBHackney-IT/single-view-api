using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary;

namespace SingleViewApi.V1.Gateways
{
    public class JigsawGateway : IJigsawGateway
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public JigsawGateway(HttpClient httpClient, string baseUrl)
        {
            this._httpClient = httpClient;
            this._baseUrl = baseUrl;
        }

        public async Task GetAuthToken()
        {
            //logic here to retrieve credentials

            var tokens = await GetCsrfTokens();

            //gather auth credentials and post
            var authCredentials = new JigsawAuthCredentials()
            {
                Email = "test",
                Password = "Test",
                RequestVerificationToken = tokens.Token
            };

            var json = JsonSerializer.Serialize(authCredentials);

            var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl);

            request.Content = new StringContent(json);

            request.Headers.Add("Cookie", String.Join("; ", tokens.Cookies));

            //Jigsaw responds with a token in the response
            var response = await _httpClient.SendAsync(request);

            //logic here to post to Redis

        }

        private async Task<CsrfTokenResponse> GetCsrfTokens()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl);
            var response = await _httpClient.SendAsync(request);
            return new CsrfTokenResponse() { Token = "test", Cookies = new List<string>() { "test" } };
        }
    }
}
