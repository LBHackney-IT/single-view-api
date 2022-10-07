using System.Threading.Tasks;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/v1/getJigsawCases")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class JigsawCasesController : BaseController
    {
        private readonly IGetJigsawCasesByCustomerIdUseCase _getJigsawCasesByCustomerIdUseCase;

        public JigsawCasesController(IGetJigsawCasesByCustomerIdUseCase getJigsawCasesByCustomerIdUseCase)
        {
            _getJigsawCasesByCustomerIdUseCase = getJigsawCasesByCustomerIdUseCase;
        }

        //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-SingleViewApi/wiki/Controllers-and-Response-Objects)
        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(CasesResponseObject), StatusCodes.Status200OK)]
        [LogCall]
        [HttpGet]
        public async Task<IActionResult> GetCasesByCustomerId([FromQuery] string id, string redisId, [FromHeader] string authorization)
        {
            var jigsawCase = await _getJigsawCasesByCustomerIdUseCase.Execute(id, redisId, authorization);
            if (jigsawCase == null)
            {
                return NotFound(null);
            }

            return Ok(jigsawCase);
        }
    }
}
