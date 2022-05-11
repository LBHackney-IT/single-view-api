using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SingleViewApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/searchJigsaw")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class JigsawSearchController : BaseController
    {

        private readonly IGetJigsawCustomersUseCase _getJigsawCustomersUse;
        public JigsawSearchController(IGetJigsawCustomersUseCase getJigsawCustomersUseCase)
        {

            _getJigsawCustomersUse = getJigsawCustomersUseCase;
        }


        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]

        [HttpGet]
        [LogCall(LogLevel.Information)]
        public IActionResult GetJigsawCustomers([FromQuery] string firstName, string lastName, [FromHeader] string bearerToken)
        {
            return Ok(_getJigsawCustomersUse.Execute(firstName, lastName, bearerToken).Result);
        }

    }
}
