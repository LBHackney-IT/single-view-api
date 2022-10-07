using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hackney.Shared.Tenure.Boundary.Response;
using Newtonsoft.Json;

namespace SingleViewApi.V1.Gateways;

public class TenureGateway : ITenureGateway
{
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;

    public TenureGateway(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
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
