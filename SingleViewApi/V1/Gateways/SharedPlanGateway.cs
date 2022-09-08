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

    public SharedPlanGateway(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }

    public async Task<SharedPlanResponseObject> GetSharedPlans(GetSharedPlanRequest getSharedPlanRequest, string userToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/plans/find");
        request.Headers.Add("Authorization", userToken);
        request.Content = new StringContent(JsonConvert.SerializeObject(getSharedPlanRequest), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

#nullable enable
        SharedPlanResponseObject? sharedPlans = null;
#nullable enable

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonBody = response.Content.ReadAsStringAsync().Result;
            sharedPlans = JsonConvert.DeserializeObject<SharedPlanResponseObject>(jsonBody);
        }

        return sharedPlans;
    }
}
