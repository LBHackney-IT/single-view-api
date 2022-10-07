using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class CreateNoteUseCase : ICreateNoteUseCase
{
    private readonly INotesGateway _gateway;

    public CreateNoteUseCase(INotesGateway gateway)
    {
        _gateway = gateway;
    }

    [LogCall]
    public async Task<NotesApiResponseObject> Execute(CreateNoteRequest createNoteRequest, string userToken)
    {
        return await _gateway.CreateNote(createNoteRequest, userToken);
    }
}
