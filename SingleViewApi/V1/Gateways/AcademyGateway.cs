using System;
using System.Collections.Generic;
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
    private readonly string _apiKey;

    public AcademyGateway(HttpClient httpClient, string baseUrl, string apiKey)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
        _apiKey = apiKey;
    }

    [LogCall]
    public async Task<CouncilTaxSearchResponseObject> GetCouncilTaxAccountsByCustomerName(string firstName, string lastName, string userToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{_baseUrl}/council-tax/search?firstName={firstName}&lastName={lastName}");

        request.Headers.Add("Authorization", userToken);
        request.Headers.Add("x-api-key", _apiKey);

        var response = await _httpClient.SendAsync(request);

#nullable enable
        CouncilTaxSearchResponseObject? results = null;
#nullable disable

        if (response.StatusCode == HttpStatusCode.OK)
        {

            var jsonBody = response.Content.ReadAsStringAsync().Result;

            results = JsonConvert.DeserializeObject<CouncilTaxSearchResponseObject>(jsonBody);

        }

        return results;
    }

    [LogCall]
    public async Task<HousingBenefitsSearchResponseObject> GetHousingBenefitsAccountsByCustomerName(string firstName, string lastName, string userToken)
    {
#nullable enable
        HousingBenefitsSearchResponseObject? results = null;
#nullable disable

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{_baseUrl}/benefits/search?firstName={firstName}&lastName={lastName}");
        request.Headers.Add("Authorization", userToken);
        request.Headers.Add("x-api-key", _apiKey);

        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonBody = response.Content.ReadAsStringAsync().Result;
            results = JsonConvert.DeserializeObject<HousingBenefitsSearchResponseObject>(jsonBody);
        }

        return results;
    }

    [LogCall]
    public async Task<CouncilTaxRecordResponseObject> GetCouncilTaxAccountByAccountRef(string accountRef, string userToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/council-tax/{accountRef}");
        request.Headers.Add("Authorization", userToken);
        request.Headers.Add("x-api-key", _apiKey);
        var response = await _httpClient.SendAsync(request);

#nullable enable
        CouncilTaxRecordResponseObject? result = null;
#nullable disable

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonBody = response.Content.ReadAsStringAsync().Result;
            result = JsonConvert.DeserializeObject<CouncilTaxRecordResponseObject>(jsonBody);
        }

        return result;
    }

    [LogCall]
    public async Task<HousingBenefitsRecordResponseObject> GetHousingBenefitsAccountByAccountRef(string accountRef, string userToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/benefits/{accountRef}");
        request.Headers.Add("Authorization", userToken);
        request.Headers.Add("x-api-key", _apiKey);
        var response = await _httpClient.SendAsync(request);

#nullable enable
        HousingBenefitsRecordResponseObject? result = null;
#nullable disable

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonBody = response.Content.ReadAsStringAsync().Result;
            result = JsonConvert.DeserializeObject<HousingBenefitsRecordResponseObject>(jsonBody);
        }

        return result;
    }

    [LogCall]
    public async Task<List<AcademyNotesResponseObject>> GetHousingBenefitsNotes(string accountRef, string userToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/benefits/{accountRef}/notes");
        request.Headers.Add("Authorization", userToken);
        request.Headers.Add("x-api-key", _apiKey);
        var response = await _httpClient.SendAsync(request);

#nullable enable
        List<AcademyNotesResponseObject>? result = null;
#nullable disable

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonBody = response.Content.ReadAsStringAsync().Result;
            result = JsonConvert.DeserializeObject<List<AcademyNotesResponseObject>>(jsonBody);
            //comment
        }

        return result;
    }

    [LogCall]
    public async Task<List<AcademyNotesResponseObject>> GetCouncilTaxNotes(string councilTaxId, string userToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/council-tax/{councilTaxId}/notes");
        request.Headers.Add("Authorization", userToken);
        request.Headers.Add("x-api-key", _apiKey);
        var response = await _httpClient.SendAsync(request);

#nullable enable
        List<AcademyNotesResponseObject>? result = null;
#nullable disable

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonBody = response.Content.ReadAsStringAsync().Result;
            result = JsonConvert.DeserializeObject<List<AcademyNotesResponseObject>>(jsonBody);
        }

        return result;
    }
}
