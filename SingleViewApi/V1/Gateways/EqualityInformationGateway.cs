using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using Newtonsoft.Json;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Gateways;

public class EqualityInformationGateway : IEqualityInformationGateway
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public EqualityInformationGateway(HttpClient httpClient, string baseUrl)
    {
        this._httpClient = httpClient;
        this._baseUrl = baseUrl;
    }
    [LogCall]
    public async Task<EqualityInformationResponseObject> GetEqualityInformationById(string id, string userToken)
    {
        Console.WriteLine("--- DEBUG -- GETTING EQUALITY INFO IN GATEWAY");
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/equality-information?targetId={id}");
        request.Headers.Add("Authorization", userToken);

        var response = await _httpClient.SendAsync(request);

#nullable enable
        EqualityInformationResponseObject? equalityInformation = null;
#nullable disable

        if (response.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine("--- DEBUG - GOT EQUALITY INFO OK");
            var jsonBody = response.Content.ReadAsStringAsync().Result;
            equalityInformation = JsonConvert.DeserializeObject<EqualityInformationResponseObject>(jsonBody);
        }

        return equalityInformation;
    }
}
