using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hackney.Shared.Person;
using Newtonsoft.Json;
using SingleViewApi.V1.Gateways.Interfaces;

namespace SingleViewApi.V1.Gateways;

public class PersonGateway : IPersonGateway
{
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;

    public PersonGateway(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl;
    }

    public async Task<Person> GetPersonById(string id, string userToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/persons/{id}");
        request.Headers.Add("Authorization", userToken);

        var response = await _httpClient.SendAsync(request);

#nullable enable
        Person? person = null;
#nullable disable

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var jsonBody = response.Content.ReadAsStringAsync().Result;
            person = JsonConvert.DeserializeObject<Person>(jsonBody);
        }

        return person;
    }
}
