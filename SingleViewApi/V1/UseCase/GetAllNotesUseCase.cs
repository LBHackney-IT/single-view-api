using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Newtonsoft.Json;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.UseCase
{
    public class GetAllNotesUseCase : IGetAllNotesUseCase
    {
        private readonly IGetNotesUseCase _getNotesUseCase;
        private readonly IGetJigsawNotesUseCase _getJigsawNotesUseCase;
        public GetAllNotesUseCase(IGetNotesUseCase getNotesUseCase, IGetJigsawNotesUseCase getJigsawNotesUseCase)
        {
            _getNotesUseCase = getNotesUseCase;
            _getJigsawNotesUseCase = getJigsawNotesUseCase;
        }

        [LogCall]
        public async Task<NotesResponse> Execute(string systemIds, string userToken, string redisKey, string paginationToken, int pageSize)
        {
            var systemIdList = new SystemIdList()
            {
                SystemIds = JsonConvert.DeserializeObject<List<SystemId>>(systemIds)
            };
            if (systemIdList.SystemIds == null) return null;

            var notes = new List<NoteResponseObject>();
            foreach (var systemId in systemIdList.SystemIds)
            {
                List<NoteResponseObject> useCaseResponse;
                if (systemId.SystemName == DataSource.Jigsaw)
                {
                    if (redisKey == null)
                    {
                        systemId.Error = SystemId.UnauthorisedMessage;
                        continue;
                    }
                    useCaseResponse = await _getJigsawNotesUseCase.Execute(systemId.Id, redisKey);
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
