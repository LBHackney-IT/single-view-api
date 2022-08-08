using System;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.UseCase.Interfaces;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Gateways.Interfaces;

namespace SingleViewApi.V1.Controllers
{
    [ApiController]
    //TODO: Rename to match the APIs endpoint
    [Route("api/v1/customers")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    //TODO: rename class to match the API name
    public class CustomerController : BaseController
    {
        private readonly ICreateCustomerUseCase _customerUseCase;
        private readonly IGetCustomerByIdUseCase _getCustomerByIdUseCase;
        private readonly ICustomerGateway _gateway;

        public CustomerController(IGetCustomerByIdUseCase getCustomerByIdUseCase, ICreateCustomerUseCase customerUseCase, ICustomerGateway gateway)
        {
            _getCustomerByIdUseCase = getCustomerByIdUseCase;
            _customerUseCase = customerUseCase;
            _gateway = gateway;
        }

        //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-SingleViewApi/wiki/Controllers-and-Response-Objects)
        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(MergedCustomerResponseObject), StatusCodes.Status200OK)]

        [HttpGet]
        [LogCall(LogLevel.Information)]
        public IActionResult GetCustomer([FromQuery] Guid id, string redisId, [FromHeader] string authorization)
        {
            return Ok(_getCustomerByIdUseCase.Execute(id, authorization, redisId));
        }

        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [HttpPost]
        [LogCall(LogLevel.Information)]
        public IActionResult SaveCustomer([FromBody] CreateCustomerRequest customer)
        {
            return Ok(_customerUseCase.Execute(customer).Id);
        }

        [HttpDelete]
        [LogCall(LogLevel.Information)]
        public IActionResult DeleteCustomer([FromQuery] Guid id, string redisId, [FromHeader] string authorization)
        {
            return Ok(_gateway.Delete(id));
        }
    }
}
