using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using Newtonsoft.Json;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;

namespace SingleViewApi.V1.Gateways;

public class EqualityInformationGateway : IEqualityInformationGateway
{
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;

    public EqualityInformationGateway(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }

    [LogCall]
    public async Task<EqualityInformationResponseObject> GetEqualityInformationById(string id, string userToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/equality-information?targetId={id}");
        request.Headers.Add("Authorization", userToken);

        var response = await _httpClient.SendAsync(request);

#nullable enable
        EqualityInformationResponseObject? equalityInformation = null;
#nullable disable

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonBody = response.Content.ReadAsStringAsync().Result;
            equalityInformation = JsonConvert.DeserializeObject<EqualityInformationResponseObject>(jsonBody);
        }

        return equalityInformation;
    }
}
