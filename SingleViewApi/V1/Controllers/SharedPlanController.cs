using System;
using System.Net.Http.Json;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

        /// <response code="200">Successfully created shared-plan</response>
        /// <response code="400">Bad request</response>
        [ProducesResponseType(typeof(CreateSharedPlanResponseObject), StatusCodes.Status200OK)]
        [HttpPost]
        [LogCall(LogLevel.Information)]
        public IActionResult CreateSharedPlan([FromBody] CreateSharedPlanRequest createSharedPlanRequest)
        {
            var result = _createSharedPlanUseCase.Execute(createSharedPlanRequest).Result;
            if (result == null)
            {
                return BadRequest();
            }

            // TODO: This should be made into a Created() response & use the Location header on FE for redirect
            Response.Headers.Add("Access-Control-Expose-Headers", "Location");

            // string response = JsonConvert.SerializeObject(
            //     new { sharedPlanUrl = sharedPlanUrl }
            // );
            return Ok(result);
        }
    }
}
