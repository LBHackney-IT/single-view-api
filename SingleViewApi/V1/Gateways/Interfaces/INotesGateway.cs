using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Gateways.Interfaces
{
    public interface INotesGateway
    {
        Task<List<NotesApiResponseObject>> GetAllById(string targetId, string userToken, string paginationToken = null, int pageSize = 0);

        Task<NotesApiResponseObject> CreateNote(CreateNoteRequest createNoteRequest, string userToken);
    }
}
