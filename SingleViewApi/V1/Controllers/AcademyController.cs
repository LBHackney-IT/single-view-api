using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SingleViewApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class AcademyController : BaseController
    {
        private readonly IGetCouncilTaxAccountByAccountRefUseCase _getCouncilTaxAccountByAccountRefUseCase;
        private readonly IGetHousingBenefitsAccountByAccountRefUseCase _getHousingBenefitsAccountByAccountRefUseCase;

        public AcademyController(IGetCouncilTaxAccountByAccountRefUseCase getCouncilTaxAccountByAccountRefUseCase, IGetHousingBenefitsAccountByAccountRefUseCase getHousingBenefitsAccountByAccountRefUseCase)
        {
            _getCouncilTaxAccountByAccountRefUseCase = getCouncilTaxAccountByAccountRefUseCase;
            _getHousingBenefitsAccountByAccountRefUseCase = getHousingBenefitsAccountByAccountRefUseCase;
        }

        //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-SingleViewApi/wiki/Controllers-and-Response-Objects)
        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(CustomerResponseObject), StatusCodes.Status200OK)]
        [HttpGet("council-tax")]
        [HttpGet("academy/council-tax")]
        [LogCall(LogLevel.Information)]
        public IActionResult GetCouncilTaxAccount([FromQuery] string id, [FromHeader] string authorization)
        {
            return Ok(_getCouncilTaxAccountByAccountRefUseCase.Execute(id, authorization).Result);
        }

        //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-SingleViewApi/wiki/Controllers-and-Response-Objects)
        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(CustomerResponseObject), StatusCodes.Status200OK)]
        [HttpGet("academy/benefits")]
        [LogCall(LogLevel.Information)]
        public IActionResult GetHousingBenefitsAccount([FromQuery] string id, [FromHeader] string authorization)
        {
            return Ok(_getHousingBenefitsAccountByAccountRefUseCase.Execute(id, authorization).Result);
        }
    }
}
