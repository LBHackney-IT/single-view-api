using System;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ServiceStack;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/v1/sharedPlan")]
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

            var result = _createSharedPlanUseCase.Execute(createSharedPlanRequest).Result;
            if (result == null)
            {
                return BadRequest();
            }
            string baseUrl = Environment.GetEnvironmentVariable("SHARED_PLAN_API");
            if (!StringExtensions.IsNullOrEmpty(baseUrl))
            {
                baseUrl.Replace("/api", "");
            }
            string sharedPlanUrl = baseUrl + "/plans/" + result.Id;

            // TODO: This should be made into a Created() response & use the Location header on FE for redirect
            // Response.Headers.Add("Access-Control-Expose-Headers", "Location");

            return Ok(sharedPlanUrl);
        }
    }
}
