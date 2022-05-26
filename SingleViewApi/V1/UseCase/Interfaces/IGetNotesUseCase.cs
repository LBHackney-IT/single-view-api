using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces
{
    public interface IGetNotesUseCase
    {
        Task<List<NoteResponseObject>> Execute(string targetId, string userToken, string paginationToken = null, int pageSize = 0);
    }
}
