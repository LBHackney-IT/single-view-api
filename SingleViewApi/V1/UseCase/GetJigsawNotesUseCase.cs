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
        public GetJigsawNotesUseCase(IJigsawGateway jigsawGateway, IGetJigsawAuthTokenUseCase getJigsawAuthTokenUseCase)
        {
            _gateway = jigsawGateway;
            _getAuthTokenUseCase = getJigsawAuthTokenUseCase;
        }

        [LogCall]
        public async Task<List<NoteResponseObject>> Execute(string customerId, string redisKey)
        {
            var token = await _getAuthTokenUseCase.Execute(redisKey);
            var gatewayResponse = await _gateway.GetCustomerNotesByCustomerId(customerId, token);
            if (gatewayResponse == null) return null;

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
                        DataSource = DataSource.Jigsaw
                    }
                );
            }
            return notes;
        }
    }
}
