using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Gateways
{
    public class NotesGateway : INotesGateway
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        public NotesGateway(HttpClient httpClient, string baseUrl)
        {
            this._httpClient = httpClient;
            this._baseUrl = baseUrl;
        }

        public async Task<List<NoteResponseObject>> GetAllById(string id, string userToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/notes?targetId={id}");
            request.Headers.Add("Authorization", userToken);

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) return null;

            var jsonBody = response.Content.ReadAsStringAsync().Result;
            var results = JsonConvert.DeserializeObject<NotesResultsResponseObject>(jsonBody);

            return results.Results;
        }
    }
}
