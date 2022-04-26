using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary;

namespace SingleViewApi.V1.UseCase
{
    public class GetAllNotesByIdUseCase : IGetAllNotesByIdUseCase
    {
        private readonly INotesGateway _gateway;

        public GetAllNotesByIdUseCase(INotesGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<NotesResponse> Execute(string id, string userToken, string paginationToken, int pageSize)
        {
            // TODO: Handle different system IDs
            var personApiId = new SystemId() { SystemName = "PersonApi", Id = id };

            var noteResponseObjects = await _gateway.GetAllById(id, userToken, paginationToken, pageSize);

            var response = new NotesResponse() {Notes = noteResponseObjects, SystemIds = new List<SystemId>() { personApiId }};

            return response;
        }
    }
}
