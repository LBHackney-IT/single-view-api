using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SingleViewApi.V1.Controllers
{
    [ApiController]
    //TODO: Rename to match the APIs endpoint
    [Route("api/v1/authorise")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    //TODO: rename class to match the API name
    public class JigsawController : BaseController
    {
        private readonly IGetJigsawAuthTokenUseCase _getJigsawAuthTokenUseCase;
        public JigsawController(IGetJigsawAuthTokenUseCase getJigsawAuthTokenUseCase)
        {
            _getJigsawAuthTokenUseCase = getJigsawAuthTokenUseCase;
        }

        //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-SingleViewApi/wiki/Controllers-and-Response-Objects)
        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]

        [HttpGet]
        [LogCall(LogLevel.Information)]
        public IActionResult GetJigsawAuthToken([FromQuery] string username,[FromHeader] string authorization)

        {
            return Ok(_getJigsawAuthTokenUseCase.Execute(username, authorization).Result);
        }
    }
}
