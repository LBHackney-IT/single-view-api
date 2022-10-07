using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces;

public interface IGetCouncilTaxNotesUseCase
{
    Task<List<NoteResponseObject>> Execute(string accountRef, string userToken);
}
