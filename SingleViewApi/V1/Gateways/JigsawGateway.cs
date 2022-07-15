using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AngleSharp;
using Hackney.Core.Logging;
using Newtonsoft.Json;
using ServiceStack;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.Helpers;

namespace SingleViewApi.V1.Gateways
{
    public class JigsawGateway : IJigsawGateway
    {

        private readonly string _authUrl;
        private readonly string _customerBaseUrl;
        private readonly string _homelessnessBaseUrl;
        private readonly string _accommodationBaseUrl;
        private readonly HttpClient _httpClient;
        public JigsawGateway(HttpClient httpClient, string authUrl, string customerBaseUrl, string homelessnessBaseUrl, string accommodationBaseUrl)
        {
            _authUrl = authUrl;
            _customerBaseUrl = customerBaseUrl;
            _homelessnessBaseUrl = homelessnessBaseUrl;
            _accommodationBaseUrl = accommodationBaseUrl;
            _httpClient = httpClient;
        }



        public async Task<AuthGatewayResponse> GetAuthToken(JigsawCredentials credentials)

        {
            try
            {
                CookieContainer cookies = new CookieContainer();
                var handler = new HttpClientHandler();
                handler.CookieContainer = cookies;
                var client = new HttpClient(handler) { BaseAddress = _httpClient.BaseAddress };

                var tokens = await GetCsrfTokens();

                var request = new HttpRequestMessage(HttpMethod.Post, _authUrl);

                var jigsawCredentials = new List<KeyValuePair<string, string>>
                {
                    new ("Email", credentials.Username),
                    new ("Password", credentials.Password),
                    new ("__RequestVerificationToken", tokens.Token)
                };

                FormUrlEncodedContent form = new FormUrlEncodedContent(jigsawCredentials);

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

                return new AuthGatewayResponse() { Token = bearerToken, ExceptionMessage = null };
            }
            catch (Exception e)
            {
                return new AuthGatewayResponse() { Token = null, ExceptionMessage = e.ToString() };
            }

        }

        public async Task<List<JigsawCustomerSearchApiResponseObject>> GetCustomers(string firstName, string lastName, string bearerToken)

        {
            var requestUrl = $"{_customerBaseUrl}/customerSearch?search={firstName}%20{lastName}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            request.Headers.Add("Authorization", $"Bearer {bearerToken}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);

#nullable enable
            List<JigsawCustomerSearchApiResponseObject>? searchResults = null;
#nullable disable

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonBody = response.Content.ReadAsStringAsync().Result;

                searchResults = JsonConvert.DeserializeObject<List<JigsawCustomerSearchApiResponseObject>>(jsonBody);

            }
            return searchResults;

        }

        [LogCall]
        public async Task<JigsawCustomerResponseObject> GetCustomerById(string id, string bearerToken)
        {
            var requestUrl = $"{_customerBaseUrl}/customerOverview/{id}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            request.Headers.Add("Authorization", $"Bearer {bearerToken}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);

#nullable enable
            JigsawCustomerResponseObject? customer = null;
#nullable disable

            if (response.StatusCode == HttpStatusCode.OK)
            {

                try
                {
                    var jsonBody = response.Content.ReadAsStringAsync().Result;
                    customer = JsonConvert.DeserializeObject<JigsawCustomerResponseObject>(jsonBody);

                }
                catch (Exception e)
                {
                    Console.WriteLine($"------ DEBUG ----- CustomerById Gateway -- Exception is {e.ToString()}");
                }



            }
            return customer;
        }

        public async Task<List<JigsawNotesResponseObject>> GetCustomerNotesByCustomerId(string id, string bearerToken)
        {
            var requestUrl = $"{_customerBaseUrl}/customer/{id}/notes";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            request.Headers.Add("Authorization", $"Bearer {bearerToken}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);

#nullable enable
            List<JigsawNotesResponseObject>? notes = null;
#nullable disable

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonBody = response.Content.ReadAsStringAsync().Result;

                notes = JsonConvert.DeserializeObject<List<JigsawNotesResponseObject>>(jsonBody);

            }

            return notes;
        }

        public async Task<JigsawCasesResponseObject> GetCasesByCustomerId(string id, string bearerToken)
        {
            var requestUrl = $"{_homelessnessBaseUrl}/casecheck/{id}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            request.Headers.Add("Authorization", $"Bearer {bearerToken}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);

#nullable enable
            JigsawCasesResponseObject? cases = null;
#nullable disable

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonBody = response.Content.ReadAsStringAsync().Result;
                cases = JsonConvert.DeserializeObject<JigsawCasesResponseObject>(jsonBody);
            }
            return cases;
        }

        public async Task<JigsawCaseOverviewResponseObject> GetCaseOverviewByCaseId(string caseId, string bearerToken)
        {
            var requestUrl = $"{_homelessnessBaseUrl}/caseoverview/{caseId}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            request.Headers.Add("Authorization", $"Bearer {bearerToken}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);

#nullable enable
            JigsawCaseOverviewResponseObject? caseOverview = null;
#nullable disable

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonBody = response.Content.ReadAsStringAsync().Result;

                caseOverview = JsonConvert.DeserializeObject<JigsawCaseOverviewResponseObject>(jsonBody);

            }
            return caseOverview;

        }

        public async Task<JigsawCasePlacementInformationResponseObject> GetCaseAccommodationPlacementsByCaseId(string caseId, string bearerToken)
        {
            var requestUrl = $"{_accommodationBaseUrl}/CaseAccommodationPlacements?caseId={caseId}";
            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            request.Headers.Add("Authorization", $"Bearer {bearerToken}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);

#nullable enable
            JigsawCasePlacementInformationResponseObject? placementInfo = null;
#nullable disable

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonBody = response.Content.ReadAsStringAsync().Result;

                placementInfo = JsonConvert.DeserializeObject<JigsawCasePlacementInformationResponseObject>(jsonBody);

            }
            return placementInfo;
        }




        private async Task<CsrfTokenResponse> GetCsrfTokens()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _authUrl);
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
