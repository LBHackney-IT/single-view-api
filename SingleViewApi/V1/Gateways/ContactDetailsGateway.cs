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
        // https://5jgh1groc6.execute-api.eu-west-2.amazonaws.com/staging/api/v2     /contactDetails?targetId=99a2e3f8-cdc9-d5b3-5999-84df1f417154

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
