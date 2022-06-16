using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SingleViewApi.V1.Gateways;

public class EqualityInformationGateway: IEqualityInformationGateway
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public EqualityInformationGateway(HttpClient httpClient, string baseUrl)
    {
        this._httpClient = httpClient;
        this._baseUrl = baseUrl;
    }

    public async Task<dynamic> GetEqualityInformationById(string id, string userToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/residentVulnerabilities/{id}");
        request.Headers.Add("Authorization", userToken);

        var response = await _httpClient.SendAsync(request);

        #nullable enable
        dynamic? equalityInformation = null;
        #nullable disable

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonBody = response.Content.ReadAsStringAsync().Result;
            equalityInformation = JsonConvert.DeserializeObject<dynamic>(jsonBody);
        }

        return equalityInformation;
    }
}
