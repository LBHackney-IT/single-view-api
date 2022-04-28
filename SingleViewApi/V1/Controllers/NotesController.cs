using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SingleViewApi.V1.Boundary;

namespace SingleViewApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/notes")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class NotesController : BaseController
    {
        private readonly IGetAllNotesByIdUseCase _getAllNotesByIdUseCase;
        public NotesController(IGetAllNotesByIdUseCase getAllNotesByIdUseCase)
        {
            _getAllNotesByIdUseCase = getAllNotesByIdUseCase;
        }

        /// <response code="200">Successfully retrieved notes from available data sources</response>
        [ProducesResponseType(typeof(NotesResponse), StatusCodes.Status200OK)]
        [HttpGet]
        [LogCall(LogLevel.Information)]

        public IActionResult ListNotes([FromQuery] string systemIds, [FromHeader] string authorization)
        {
            return Ok(_getAllNotesByIdUseCase.Execute(systemIds, authorization).Result);
        }
    }
}
