using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Newtonsoft.Json;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase
{
    public class GetAllNotesByIdUseCase : IGetAllNotesByIdUseCase
    {
        public const string NotFound = "Not found";
        private readonly INotesGateway _gateway;
        public GetAllNotesByIdUseCase(INotesGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<NotesResponse> Execute(string systemIds, string userToken, string paginationToken, int pageSize)
        {
            var notes = new NoteResponseObjectList()
            {
                NoteResponseObjects = new List<NoteResponseObject>()
            };

            var systemIdList = new SystemIdList()
            {
                SystemIds = JsonConvert.DeserializeObject<List<SystemId>>(systemIds)
            };

            foreach (var systemId in systemIdList.SystemIds)
            {
                var noteResponseList = await _gateway.GetAllById(systemId.Id, userToken, paginationToken, pageSize);

                if (noteResponseList == null)
                {
                    systemId.Error = NotFound;
                }
                else
                {
                    notes.NoteResponseObjects.AddRange(noteResponseList.NoteResponseObjects);
                }
            }

            notes.SortByCreatedAtDescending();

            return new NotesResponse() { Notes = notes, SystemIds = systemIdList.SystemIds };
        }
    }
}
