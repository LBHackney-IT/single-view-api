using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SingleViewApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/authorise")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class JigsawController : BaseController
    {
        private readonly IGetJigsawAuthTokenUseCase _getJigsawAuthTokenUseCase;
        public JigsawController(IGetJigsawAuthTokenUseCase getJigsawAuthTokenUseCase)
        {
            _getJigsawAuthTokenUseCase = getJigsawAuthTokenUseCase;

        }


        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]

        [HttpPost]
        [LogCall(LogLevel.Information)]
        public IActionResult GetJigsawAuthToken([FromBody] string hashedUsername, string hashedPassword)

        {
            return Ok(_getJigsawAuthTokenUseCase.Execute(hashedUsername, hashedPassword).Result);
        }


    }
}
