using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        public async Task<dynamic> SearchBySearchText(string searchText, string userToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/persons?searchText={searchText}");

            request.Headers.Add("Authorization", userToken);

            var response = await _httpClient.SendAsync(request);

            #nullable enable
            dynamic? searchResults = null;
            #nullable disable

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonBody = response.Content.ReadAsStringAsync().Result;

                searchResults = JsonConvert.DeserializeObject<dynamic>(jsonBody);
            }

            return searchResults;
        }
    }
}
