using System.Threading.Tasks;
using SingleViewApi.V1.Boundary;

namespace SingleViewApi.V1.UseCase.Interfaces
{
    public interface IGetAllNotesByIdUseCase
    {
        Task<NotesResponse> Execute(string systemIds, string userToken, string paginationToken = null, int pageSize = 0);
    }
}
