using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.Gateways.Interfaces;

namespace SingleViewApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/postgres")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class PostgresController : BaseController
    {
        private readonly IDataSourceGateway _dataSourceGateway;

        public PostgresController(IDataSourceGateway dataSourceGateway)
        {
            _dataSourceGateway = dataSourceGateway;

        }

        //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-SingleViewApi/wiki/Controllers-and-Response-Objects)
        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]

        [HttpGet]
        [Route("GetAll")]
        [LogCall(LogLevel.Information)]
        public IActionResult GetAll()
        {
            var res = _dataSourceGateway.GetAll();
            return Ok(res);
        }
        [HttpGet]
        [Route("Get")]
        [LogCall(LogLevel.Information)]
        public IActionResult Get([FromQuery] int id)
        {
            var value = _dataSourceGateway.GetEntityById(id);
            return Ok(value);
        }
    }
}
