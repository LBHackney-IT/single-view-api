using System.Threading.Tasks;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase
{
    public class CreateNoteUseCase : ICreateNoteUseCase
    {
        private readonly INotesGateway _gateway;
        public CreateNoteUseCase(INotesGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<NoteResponseObject> Execute(string targetId, string userToken, CreateNoteRequest createNoteRequest)
        {
            return await _gateway.CreateNote(targetId, userToken, createNoteRequest);
        }
    }
}
