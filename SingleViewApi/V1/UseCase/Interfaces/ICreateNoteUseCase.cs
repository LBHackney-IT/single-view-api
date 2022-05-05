using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces
{
    public interface ICreateNoteUseCase
    {
        Task<NoteResponseObject> Execute(CreateNoteRequest createNoteRequest, string userToken);
    }
}
