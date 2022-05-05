using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using ServiceStack;
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

        public async Task<string> GetAuthToken(string email, string password)
        {
            //logic here to retrieve credentials. For now I am going to store creds as env variables
            //so we know the logic works, before we integrate with redis

            var tokens = await GetCsrfTokens();

            //gather auth credentials and post
            var authCredentials = new JigsawAuthCredentials()
            {
                Email = email,
                Password = password,
                RequestVerificationToken = tokens.Token
            };

            var json = JsonSerializer.Serialize(authCredentials);

            var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl);

            request.Content = new StringContent(json);

            request.Headers.Add("Cookie", String.Join("; ", tokens.Cookies));

            //Jigsaw responds with a token in the response
            var response = await _httpClient.SendAsync(request);

            var bearerToken = String.Empty;

            foreach (string header in response.Headers.GetValues("set-cookie"))
            {
                Regex r = new Regex("/access_token=([^;]*)/");

                Match match = r.Match(header);

                if (match.Success)
                {
                    bearerToken = match.Value;
                }

            }

            //this should post to redis, but for now just return the token
            return bearerToken;
        }

        private async Task<CsrfTokenResponse> GetCsrfTokens()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl);
            var response = await _httpClient.SendAsync(request);

            var cookies = response.Headers.GetValues("set-cookie").Map((cookie) => cookie.Split(";")[0]);

            var parser = new HtmlParser();
            var document = parser.ParseDocument(response.Content.ToString());

            var token = String.Empty;

            foreach (IElement element in document.QuerySelectorAll("input[name='__RequestVerificationToken']"))
            {
                token = element.NodeValue;
            }


            return new CsrfTokenResponse() { Token = token, Cookies = cookies };
        }


    }
}
