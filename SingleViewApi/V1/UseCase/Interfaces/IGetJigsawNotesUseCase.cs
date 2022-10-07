using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IGetJigsawNotesUseCase
{
    Task<List<NoteResponseObject>> Execute(string customerId, string redisKey, string userToken);
}
