using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.UseCase
{
    public class GetJigsawNotesUseCase : IGetJigsawNotesUseCase
    {
        private readonly IJigsawGateway _gateway;
        private readonly IGetJigsawAuthTokenUseCase _getAuthTokenUseCase;
        private readonly IDataSourceGateway _dataSourceGateway;

        public GetJigsawNotesUseCase(IJigsawGateway jigsawGateway, IGetJigsawAuthTokenUseCase getJigsawAuthTokenUseCase, IDataSourceGateway dataSourceGateway)
        {
            _gateway = jigsawGateway;
            _getAuthTokenUseCase = getJigsawAuthTokenUseCase;
            _dataSourceGateway = dataSourceGateway;
        }

        [LogCall]
        public async Task<List<NoteResponseObject>> Execute(string customerId, string redisKey, string userToken)
        {
            var authResponse = await _getAuthTokenUseCase.Execute(redisKey, userToken);
            if (authResponse.ExceptionMessage != null)
            {
                Console.WriteLine(authResponse.ExceptionMessage);
                return null;
            }
            var gatewayResponse = await _gateway.GetCustomerNotesByCustomerId(customerId, authResponse.Token);

            if (gatewayResponse == null) return null;

            var dataSource = _dataSourceGateway.GetEntityById(2);

            var notes = new List<NoteResponseObject>();
            foreach (var note in gatewayResponse)
            {
                notes.Add(
                    new NoteResponseObject()
                    {
                        Description = note.Content,
                        CreatedAt = note.CreatedDate,
                        Categorisation = new Categorisation() { Description = $"Jigsaw NoteTypeId: {note.NoteTypeId}" },
                        Author = new AuthorDetails() { FullName = note.OfficerName },
                        IsSensitive = note.IsSensitive,
                        IsPinned = note.IsPinned,
                        DataSourceId = note.Id.ToString(),
                        DataSource = dataSource.Name
                    }
                );
            }
            return notes;
        }
    }
}
