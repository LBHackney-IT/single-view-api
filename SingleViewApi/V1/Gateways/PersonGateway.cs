using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hackney.Shared.Person;
using Newtonsoft.Json;

namespace SingleViewApi.V1.Gateways
{
    public class PersonGateway : IPersonGateway
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public PersonGateway(HttpClient httpClient, string baseUrl)
        {
            this._httpClient = httpClient;
            this._baseUrl = baseUrl;
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
                person = JsonConvert.DeserializeObject<Person>(response.Content.ReadAsStringAsync().Result);
            }

            return person;
        }
    }
}
