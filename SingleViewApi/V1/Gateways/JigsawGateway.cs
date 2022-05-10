using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using AngleSharp;
using Microsoft.AspNetCore.Http;
using ServiceStack;
using SingleViewApi.V1.Boundary;

namespace SingleViewApi.V1.Gateways
{
    public class JigsawGateway : IJigsawGateway
    {

        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;
        //private readonly HttpContext _httpContext;

        public JigsawGateway(HttpClient httpClient, string baseUrl)
        {
            this._baseUrl = baseUrl;
            this._httpClient = httpClient;
        }

        public async Task<string> GetAuthToken(string email)
        {
            CookieContainer cookies = new CookieContainer();
            var handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            var client = new HttpClient(handler) { BaseAddress = _httpClient.BaseAddress };

            var tokens = await GetCsrfTokens();

            var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl);

            var keyPairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Email", email),
                new KeyValuePair<string, string>("Password", Environment.GetEnvironmentVariable("JIGSAW_PASSWORD")),
                new KeyValuePair<string, string>("__RequestVerificationToken", tokens.Token)
            };

            FormUrlEncodedContent form = new FormUrlEncodedContent(keyPairs);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            request.Content = form;
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            request.Headers.Add("Cookie", tokens.Cookies.Join("; "));

            await client.SendAsync(request);

            var bearerToken = String.Empty;

            IEnumerable<Cookie> responseCookies = cookies.GetCookies(_httpClient.BaseAddress);

            foreach (Cookie cookie in responseCookies)
            {
                if (cookie.Name == "access_token")
                {
                    bearerToken = cookie.Value;
                }
            }
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
