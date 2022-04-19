using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hackney.Shared.Person;

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

        public async Task<Person> GetPersonById(int id, string userToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/persons/{id}");
            request.Headers.Add("Authorization", userToken);

            var response = await _httpClient.SendAsync(request);
            var data = new Person();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                data = await response.Content.ReadFromJsonAsync<Person>();
            }

            return data;
        }
    }
}