using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hackney.Core.Logging;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Domain;
using SingleViewApi.V1.Gateways.Interfaces;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetCouncilTaxNotesUseCase : IGetCouncilTaxNotesUseCase
{
    private readonly IAcademyGateway _academyGateway;
    private readonly IDataSourceGateway _dataSourceGateway;

    public GetCouncilTaxNotesUseCase(IAcademyGateway academyGateway, IDataSourceGateway dataSourceGateway)
    {
        _academyGateway = academyGateway;
        _dataSourceGateway = dataSourceGateway;
    }

    [LogCall]
    public async Task<List<NoteResponseObject>> Execute(string accountRef, string userToken)
    {
        var gatewayResponse = await _academyGateway.GetCouncilTaxNotes(accountRef, userToken);

        if (gatewayResponse == null) return null;

        var dataSource = _dataSourceGateway.GetEntityById(3);

        var notes = new List<NoteResponseObject>();
        foreach (var note in gatewayResponse)
            notes.Add(
                new NoteResponseObject
                {
                    Description = note.Note,
                    CreatedAt = ParseNoteDate(note.Date) ?? new DateTime(),
                    Categorisation = new Categorisation { Description = "Academy Council Tax Note" },
                    Author = new AuthorDetails { FullName = note.UserId },
                    DataSource = dataSource.Name
                }
            );
        return notes;
    }

    private static DateTime? ParseNoteDate(string noteDate)
    {
        var dt = Regex.Replace(noteDate, @"\d{10}", "").TrimEnd().ToDate("dd.MM.yyyy HH:mm:ss");
        return dt;
    }
}
