using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Hackney.Shared.Tenure.Boundary.Response;

namespace SingleViewApi.V1.Gateways
{
    public class TenureGateway : ITenureGateway
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public TenureGateway(HttpClient httpClient, string baseUrl)
        {
            this._httpClient = httpClient;
            this._baseUrl = baseUrl;
        }

        public async Task<TenureResponseObject> GetTenureInformation(Guid id, string userToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/tenures/{id}");
            request.Headers.Add("Authorization", userToken);

            var response = await _httpClient.SendAsync(request);

#nullable enable
            var tenure = new TenureResponseObject();
#nullable disable

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonBody = response.Content.ReadAsStringAsync().Result;
                tenure = JsonConvert.DeserializeObject<TenureResponseObject>(jsonBody);
            }

            return tenure;
        }
    }
}
