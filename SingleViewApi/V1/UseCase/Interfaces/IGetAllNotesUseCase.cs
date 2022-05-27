using System.Threading.Tasks;
using SingleViewApi.V1.Boundary;

namespace SingleViewApi.V1.UseCase.Interfaces
{
    public interface IGetAllNotesUseCase
    {
        Task<NotesResponse> Execute(string systemIds, string userToken, string redisKey = null, string paginationToken = null, int pageSize = 0);
    }
}
