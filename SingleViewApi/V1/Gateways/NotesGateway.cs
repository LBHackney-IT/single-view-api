using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SingleViewApi.V1.Boundary.Request;
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

        public async Task<NoteResponseObjectList> GetAllById(string targetId, string userToken, string paginationToken = null, int pageSize = 0)
        {
            var requestUrl = $"{_baseUrl}/notes?targetId={targetId}";

            if (!string.IsNullOrEmpty(paginationToken))
            {
                requestUrl += $"&paginationToken={paginationToken}";
            }

            if (pageSize > 0)
            {
                requestUrl += $"&pageSize={pageSize}";
            }

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Add("Authorization", userToken);

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) return null;

            var jsonBody = response.Content.ReadAsStringAsync().Result;
            var responseObject = JsonConvert.DeserializeObject<NotesResultsResponseObject>(jsonBody);

            var noteResponseObjectList = new NoteResponseObjectList() { NoteResponseObjects = responseObject.Results };

            return noteResponseObjectList;
        }

        public async Task<NoteResponseObject> CreateNote(string targetId, string userToken, CreateNoteRequest createNoteRequest)
        {
            var requestUrl = $"{_baseUrl}/notes?targetId={targetId}";
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);

            request.Headers.Add("Authorization", userToken);
            request.Content = new StringContent(JsonConvert.SerializeObject(createNoteRequest));

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.Created) return null;

            var jsonBody = response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<NoteResponseObject>(jsonBody);
        }
    }
}
