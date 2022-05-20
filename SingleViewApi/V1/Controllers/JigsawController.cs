using System.Runtime.CompilerServices;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/search")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class JigsawController : BaseController
    {
        private readonly IGetJigsawCustomerByIdUseCase _getJigsawCustomerByIdUseCase;

        public JigsawController(IGetJigsawCustomerByIdUseCase getJigsawCustomerByIdUseCase)
        {
            _getJigsawCustomerByIdUseCase = getJigsawCustomerByIdUseCase;
        }

        //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-SingleViewApi/wiki/Controllers-and-Response-Objects)
        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(DynamicAttribute), StatusCodes.Status200OK)]
        [LogCall]
        [HttpGet]
        public IActionResult GetCustomerById([FromQuery] string id, string redisId)
        {
            return Ok(_getJigsawCustomerByIdUseCase.Execute(id, redisId));
        }
    }
}

