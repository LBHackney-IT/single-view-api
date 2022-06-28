using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SingleViewApi.V1.Gateways.Interfaces;

namespace SingleViewApi.V1.Gateways;

public class AcademyGateway: IAcademyGateway
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public AcademyGateway(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }

    public async Task<dynamic> GetCouncilTaxAccountsByCustomerName(string firstName, string lastName)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{_baseUrl}/search?firstName={firstName}&lastName={lastName}");

        var response = await _httpClient.SendAsync(request);

#nullable enable
        dynamic? results = null;
#nullable disable

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonBody = response.Content.ReadAsStringAsync().Result;


            results = JsonConvert.DeserializeObject<dynamic>(jsonBody);

        }

        return results;
    }
}
