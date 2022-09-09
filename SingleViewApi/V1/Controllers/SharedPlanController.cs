using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/sharedPlan")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class SharedPlanController : BaseController
    {
        private readonly ICreateSharedPlanUseCase _createSharedPlanUseCase;

        public SharedPlanController(ICreateSharedPlanUseCase createSharedPlanUseCase)
        {
            _createSharedPlanUseCase = createSharedPlanUseCase;
        }

        /// <response code="201">Successfully created shared-plan</response>
        /// <response code="400">Bad request</response>
        [ProducesResponseType(typeof(CreateSharedPlanResponseObject), StatusCodes.Status201Created)]
        [HttpPost]
        [LogCall(LogLevel.Information)]
        public IActionResult CreateSharedPlan([FromBody] CreateSharedPlanRequest createSharedPlanRequest)
        {
            var result = _createSharedPlanUseCase.Execute(createSharedPlanRequest);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }
    }
}
