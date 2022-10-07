using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Gateways;

public class CautionaryAlertsGateway : ICautionaryAlertsGateway
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public CautionaryAlertsGateway(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }

    public async Task<CautionaryAlertResponseObject> GetCautionaryAlertsById(string id, string userToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/cautionary-alerts/persons/{id}");
        request.Headers.Add("Authorization", userToken);

        var response = await _httpClient.SendAsync(request);

#nullable enable
        CautionaryAlertResponseObject? cautionaryAlerts = null;
#nullable disable

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonBody = response.Content.ReadAsStringAsync().Result;
            cautionaryAlerts = JsonConvert.DeserializeObject<CautionaryAlertResponseObject>(jsonBody);
        }

        return cautionaryAlerts;
    }
}
