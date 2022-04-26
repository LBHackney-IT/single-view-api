using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces
{
    public interface IGetAllNotesByIdUseCase
    {
        Task<NotesResponse> Execute(string id, string userToken, string paginationToken = null, int pageSize = 0);
    }
}
