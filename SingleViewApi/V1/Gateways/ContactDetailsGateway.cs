using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hackney.Shared.ContactDetail.Domain;
using Hackney.Shared.Person;
using Newtonsoft.Json;

namespace SingleViewApi.V1.Gateways
{
    public class ContactDetailsGateway : IContactDetailsGateway
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ContactDetailsGateway(HttpClient httpClient, string baseUrl)
        {
            this._httpClient = httpClient;
            this._baseUrl = baseUrl;
        }

        public async Task<ContactDetails> GetContactDetailsById(string id, string userToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/contactDetails?targetId={id}&includeHistoric=true");
            request.Headers.Add("Authorization", userToken);

            var response = await _httpClient.SendAsync(request);

#nullable enable
            ContactDetails? contactDetails = null;
#nullable disable

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonBody = response.Content.ReadAsStringAsync().Result;
                contactDetails = JsonConvert.DeserializeObject<ContactDetails>(jsonBody);
            }

            return contactDetails;
        }
    }
}
