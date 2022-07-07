using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Newtonsoft.Json;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.Gateways.Interfaces;

namespace SingleViewApi.V1.UseCase
{
    public class GetAllNotesUseCase : IGetAllNotesUseCase
    {
        private readonly IGetNotesUseCase _getNotesUseCase;
        private readonly IGetJigsawNotesUseCase _getJigsawNotesUseCase;
        private readonly IDataSourceGateway _dataSourceGateway;

        public GetAllNotesUseCase(IGetNotesUseCase getNotesUseCase, IGetJigsawNotesUseCase getJigsawNotesUseCase, IDataSourceGateway dataSourceGateway)
        {
            _getNotesUseCase = getNotesUseCase;
            _getJigsawNotesUseCase = getJigsawNotesUseCase;
            _dataSourceGateway = dataSourceGateway;
        }

        [LogCall]
        public async Task<NotesResponse> Execute(string systemIds, string userToken, string redisKey, string paginationToken, int pageSize)
        {
            var systemIdList = new SystemIdList()
            {
                SystemIds = JsonConvert.DeserializeObject<List<SystemId>>(systemIds)
            };
            if (systemIdList.SystemIds == null) return null;

            var dataSource = _dataSourceGateway.GetEntityById(2);

            var notes = new List<NoteResponseObject>();
            foreach (var systemId in systemIdList.SystemIds)
            {
                List<NoteResponseObject> useCaseResponse;
                if (systemId.SystemName == dataSource.Name)
                {
                    if (redisKey == null)
                    {
                        systemId.Error = SystemId.UnauthorisedMessage;
                        continue;
                    }
                    useCaseResponse = await _getJigsawNotesUseCase.Execute(systemId.Id, redisKey, userToken);
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
            var response = new NotesResponse() { Notes = notes, SystemIds = systemIdList.SystemIds };
            response.Sort();
            return response;
        }
    }
}
