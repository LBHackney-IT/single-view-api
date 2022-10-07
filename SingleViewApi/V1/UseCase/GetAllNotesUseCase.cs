using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using Newtonsoft.Json;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetAllNotesUseCase : IGetAllNotesUseCase
{
    private readonly IDataSourceGateway _dataSourceGateway;
    private readonly IGetCouncilTaxNotesUseCase _getCouncilTaxNotesUseCase;
    private readonly IGetHousingBenefitsNotesUseCase _getHousingBenefitsNotesUseCase;
    private readonly IGetJigsawNotesUseCase _getJigsawNotesUseCase;
    private readonly IGetNotesUseCase _getNotesUseCase;

    public GetAllNotesUseCase(
        IGetNotesUseCase getNotesUseCase,
        IGetJigsawNotesUseCase getJigsawNotesUseCase,
        IGetCouncilTaxNotesUseCase getCouncilTaxNotesUseCase,
        IGetHousingBenefitsNotesUseCase getHousingBenefitsNotesUseCase,
        IDataSourceGateway dataSourceGateway)
    {
        _getNotesUseCase = getNotesUseCase;
        _getJigsawNotesUseCase = getJigsawNotesUseCase;
        _getCouncilTaxNotesUseCase = getCouncilTaxNotesUseCase;
        _getHousingBenefitsNotesUseCase = getHousingBenefitsNotesUseCase;
        _dataSourceGateway = dataSourceGateway;
    }

    [LogCall]
    public async Task<NotesResponse> Execute(string systemIds, string userToken, string redisKey,
        string paginationToken, int pageSize)
    {
        var systemIdList = new SystemIdList { SystemIds = JsonConvert.DeserializeObject<List<SystemId>>(systemIds) };
        if (systemIdList.SystemIds == null) return null;

        var jigsawSource = _dataSourceGateway.GetEntityById(2);
        var councilTaxSource = _dataSourceGateway.GetEntityById(3);
        var housingBenefitsSource = _dataSourceGateway.GetEntityById(4);

        var notes = new List<NoteResponseObject>();
        foreach (var systemId in systemIdList.SystemIds)
        {
            List<NoteResponseObject> useCaseResponse;
            if (systemId.SystemName == jigsawSource.Name)
            {
                if (redisKey == null)
                {
                    systemId.Error = SystemId.UnauthorisedMessage;
                    continue;
                }

                useCaseResponse = await _getJigsawNotesUseCase.Execute(systemId.Id, redisKey, userToken);
            }
            else if (systemId.SystemName == councilTaxSource.Name)
            {
                useCaseResponse =
                    await _getCouncilTaxNotesUseCase.Execute(systemId.Id, userToken);
            }
            else if (systemId.SystemName == housingBenefitsSource.Name)
            {
                useCaseResponse =
                    await _getHousingBenefitsNotesUseCase.Execute(systemId.Id, userToken);
            }
            else
            {
                useCaseResponse =
                    await _getNotesUseCase.Execute(systemId.Id, userToken, paginationToken, pageSize);
            }

            if (useCaseResponse == null)
            {
                systemId.Error = SystemId.NotFoundMessage;
                continue;
            }

            notes.AddRange(useCaseResponse);
        }

        var response = new NotesResponse { Notes = notes, SystemIds = systemIdList.SystemIds };
        response.Sort();
        return response;
    }
}
