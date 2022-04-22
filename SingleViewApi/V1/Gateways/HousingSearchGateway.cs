using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hackney.Shared.Person;
using Newtonsoft.Json;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Gateways
{
    public class HousingSearchGateway : IHousingSearchGateway
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public HousingSearchGateway(HttpClient httpClient, string baseUrl)
        {
            this._httpClient = httpClient;
            this._baseUrl = baseUrl;
        }
        public async Task<HousingSearchApiResponseObject> GetSearchResultsBySearchText(string searchText, string userToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/search/persons?searchText={searchText}");
            request.Headers.Add("Authorization", userToken);

            var response = await _httpClient.SendAsync(request);

        #nullable enable
            HousingSearchApiResponseObject? searchResults = null;
        #nullable disable


            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonBody = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(jsonBody);
                searchResults = JsonConvert.DeserializeObject<HousingSearchApiResponseObject>(jsonBody);
            }

            return searchResults;
        }
    }
}
