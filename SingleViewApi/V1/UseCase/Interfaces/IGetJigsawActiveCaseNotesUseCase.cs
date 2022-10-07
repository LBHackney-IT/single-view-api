using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IGetJigsawActiveCaseNotesUseCase
{
    Task<List<JigsawNotesResponseObject>> Execute(string customerId, string authToken);
}
