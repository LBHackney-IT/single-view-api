using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetNotesUseCase : IGetNotesUseCase
{
    private readonly IDataSourceGateway _dataSourceGateway;
    private readonly INotesGateway _notesGateway;

    public GetNotesUseCase(INotesGateway notesGateway, IDataSourceGateway dataSourceGateway)
    {
        _notesGateway = notesGateway;
        _dataSourceGateway = dataSourceGateway;
    }

    [LogCall]
    public async Task<List<NoteResponseObject>> Execute(string targetId, string userToken, string paginationToken,
        int pageSize)
    {
        var gatewayResponse = await _notesGateway.GetAllById(targetId, userToken, paginationToken, pageSize);
        var dataSource = _dataSourceGateway.GetEntityById(1);
        if (gatewayResponse == null) return null;

        var notes = new List<NoteResponseObject>();
        foreach (var note in gatewayResponse)
            notes.Add(
                new NoteResponseObject
                {
                    Id = note.Id,
                    Title = note.Title,
                    Description = note.Description,
                    TargetType = note.TargetType,
                    TargetId = note.TargetId,
                    CreatedAt = note.CreatedAt,
                    Categorisation = note.Categorisation,
                    Author = note.Author,
                    Highlight = note.Highlight,
                    DataSourceId = note.Id.ToString(),
                    DataSource = dataSource.Name
                }
            );
        return notes;
    }
}
