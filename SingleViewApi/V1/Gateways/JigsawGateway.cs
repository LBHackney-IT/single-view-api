using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using ServiceStack;
using SingleViewApi.V1.Boundary;

namespace SingleViewApi.V1.Gateways
{
    public class JigsawGateway : IJigsawGateway
    {

        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public JigsawGateway(HttpClient httpClient, string baseUrl)
        {
            this._baseUrl = baseUrl;
            this._httpClient = httpClient;

        }

        public async Task<string> GetAuthToken(string email)
        {
            //logic here to retrieve credentials. For now I am going to store creds as env variables
            //so we know the logic works, before we integrate with redis
            var baseAddress = new Uri(_baseUrl);

           // var handler = new HttpClientHandler() { UseCookies = false };
           // var client = new HttpClient(handler) { BaseAddress = baseAddress };

            var tokens = await GetCsrfTokens();

            var authCredentials = new JigsawAuthCredentials()
            {
                Email = email,
                Password = Environment.GetEnvironmentVariable("JIGSAW_PASSWORD"),
                RequestVerificationToken = tokens.Token
            };

            var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl);

            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new StringContent(authCredentials.Email), "Email");
            form.Add(new StringContent(authCredentials.Password), "Password");
            form.Add(new StringContent(authCredentials.RequestVerificationToken), "__RequestVerificationToken");

            request.Content = form;

            request.Headers.Add("Cookie", tokens.Cookies.Join("; "));

            foreach (var cookie in tokens.Cookies)
            {
                Console.WriteLine("token being passed to set as cookie is are: {0}", cookie);
            }

            Console.WriteLine("Cookie set is {0}", JSON.stringify(request.Headers.GetValues("Cookie")));

            var response = await _httpClient.SendAsync(request);

            Console.WriteLine("Full request is {0}", JSON.stringify(request));
            Console.WriteLine("Full response is {0}", JSON.stringify(response));

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
            Console.WriteLine("Bearer token is {0}", bearerToken);
            return bearerToken;
        }

        private async Task<CsrfTokenResponse> GetCsrfTokens()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl);
            var response = await _httpClient.SendAsync(request);

            var cookies = response.Headers.GetValues("set-cookie").Map((cookie) => cookie.Split(";")[0]);

            var body = response.Content.ReadAsStringAsync().Result;

            var context = BrowsingContext.New(Configuration.Default);

            var document = await context.OpenAsync(req => req.Content(body));

            var element = document.QuerySelector("input[name='__RequestVerificationToken']");

            var token = element.Attributes[2].Value;

            return new CsrfTokenResponse() { Token = token, Cookies = cookies };
        }


    }
}
