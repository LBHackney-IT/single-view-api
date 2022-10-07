using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetJigsawActiveCaseNotesUseCase : IGetJigsawActiveCaseNotesUseCase
{
    private readonly IJigsawGateway _jigsawGateway;

    public GetJigsawActiveCaseNotesUseCase(IJigsawGateway jigsawGateway)
    {
        _jigsawGateway = jigsawGateway;
    }

    [LogCall]
    public async Task<List<JigsawNotesResponseObject>> Execute(string customerId, string authToken)
    {
        var customerCases = await _jigsawGateway.GetCasesByCustomerId(customerId, authToken);

        if (customerCases == null) return null;

        var activeCase = customerCases.Cases.FirstOrDefault(x => x.IsCurrent);

        if (activeCase == null) return null;

        var activeCaseNotes = await _jigsawGateway.GetActiveCaseNotesByCaseId(activeCase.Id.ToString(), authToken);

        return activeCaseNotes;
    }
}
