using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.Controllers;

[ApiController]
[Route("api/v1/notes")]
[Produces("application/json")]
[ApiVersion("1.0")]
public class NotesController : BaseController
{
    private readonly ICreateNoteUseCase _createNoteUseCase;
    private readonly IGetAllNotesUseCase _getAllNotesUseCase;

    public NotesController(IGetAllNotesUseCase getAllNotesByIdUseCase, ICreateNoteUseCase createNoteUseCase)
    {
        _getAllNotesUseCase = getAllNotesByIdUseCase;
        _createNoteUseCase = createNoteUseCase;
    }

    /// <response code="200">Successfully retrieved notes from available data sources</response>
    [ProducesResponseType(typeof(NotesResponse), StatusCodes.Status200OK)]
    [HttpGet]
    [LogCall(LogLevel.Information)]
    public IActionResult ListNotes([FromQuery] string systemIds, string redisId, [FromHeader] string authorization)
    {
        return Ok(_getAllNotesUseCase.Execute(systemIds, authorization, redisId).Result);
    }

    /// <response code="201">Successfully created note</response>
    /// <response code="400">Bad request</response>
    [ProducesResponseType(typeof(NoteResponseObject), StatusCodes.Status201Created)]
    [HttpPost]
    [LogCall(LogLevel.Information)]
    public IActionResult CreateNote([FromBody] CreateNoteRequest createNoteRequest, [FromHeader] string authorization)
    {
        var result = _createNoteUseCase.Execute(createNoteRequest, authorization).Result;

        if (result == null) return BadRequest(createNoteRequest.TargetId);

        return Created("api/v1/notes", result);
    }
}
