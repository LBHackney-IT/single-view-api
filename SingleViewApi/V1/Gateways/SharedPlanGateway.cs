using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;

namespace SingleViewApi.V1.Gateways;

public class SharedPlanGateway : ISharedPlanGateway
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _xApiKey;

    public SharedPlanGateway(HttpClient httpClient, string baseUrl, string xApiKey)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
        _xApiKey = xApiKey;
    }

    public async Task<SharedPlanResponseObject> GetSharedPlans(GetSharedPlanRequest getSharedPlanRequest)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/plans/find");
        request.Headers.Add("x-api-key", _xApiKey);
        request.Content = new StringContent(JsonConvert.SerializeObject(getSharedPlanRequest), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

#nullable enable
        SharedPlanResponseObject? sharedPlans = null;
#nullable disable

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonBody = response.Content.ReadAsStringAsync().Result;
            sharedPlans = JsonConvert.DeserializeObject<SharedPlanResponseObject>(jsonBody);
        }

        return sharedPlans;
    }

    public async Task<CreateSharedPlanResponseObject> CreateSharedPlan(CreateSharedPlanRequest createSharedPlanRequest)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://staging.sharedplan.hackney.gov.uk/api/plans");
        request.Headers.Add("x-api-key", _xApiKey);
        request.Content = new StringContent(JsonConvert.SerializeObject(createSharedPlanRequest), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

#nullable enable
        CreateSharedPlanResponseObject? createSharedPlanResponse = null;
#nullable disable

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonBody = response.Content.ReadAsStringAsync().Result;
            createSharedPlanResponse = JsonConvert.DeserializeObject<CreateSharedPlanResponseObject>(jsonBody);
        }

        return createSharedPlanResponse;
    }
}
