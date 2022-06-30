using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using Newtonsoft.Json;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;

namespace SingleViewApi.V1.Gateways;

public class AcademyGateway : IAcademyGateway
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public AcademyGateway(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }

    [LogCall]
    public async Task<CouncilTaxSearchResponseObject> GetCouncilTaxAccountsByCustomerName(string firstName, string lastName, string userToken)
    {

        Console.WriteLine("Making request to Academy Council Tax Endpoint");

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{_baseUrl}/council-tax/search?firstName={firstName}&lastName={lastName}");

        request.Headers.Add("Authorization", userToken);
        var response = await _httpClient.SendAsync(request);

#nullable enable
        CouncilTaxSearchResponseObject? results = null;
#nullable disable

        if (response.StatusCode == HttpStatusCode.OK)
        {
            Console.WriteLine("Response OK");
            var jsonBody = response.Content.ReadAsStringAsync().Result;


            results = JsonConvert.DeserializeObject<CouncilTaxSearchResponseObject>(jsonBody);

            Console.WriteLine($"results are {results?.ToString()}");

        }

        return results;
    }
}
