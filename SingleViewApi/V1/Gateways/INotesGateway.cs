using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Gateways
{
    public interface INotesGateway
    {
        Task<NoteResponseObjectList> GetAllById(string targetId, string userToken, string paginationToken = null, int pageSize = 0);
    }
}
