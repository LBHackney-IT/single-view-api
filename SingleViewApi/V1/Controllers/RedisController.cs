using System;
using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Gateways;

namespace SingleViewApi.V1.Controllers
{
    [ApiController]
    [Route("api/v1/redis")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class RedisController : BaseController
    {
        private readonly IRedisGateway _redisGateway;

        public RedisController(IRedisGateway redisGateway)
        {
            _redisGateway = redisGateway;

        }

        //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-SingleViewApi/wiki/Controllers-and-Response-Objects)
        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]

        [HttpGet]
        [Route("Add")]
        [LogCall(LogLevel.Information)]
        public IActionResult AddToRedis([FromQuery] string value)
        {
            var id = _redisGateway.AddValue(value, 1);
            return Ok(id);
        }
        [HttpGet]
        [Route("Get")]
        [LogCall(LogLevel.Information)]
        public IActionResult GetFromRedis([FromQuery] string id)
        {
            var value = _redisGateway.GetValue(id);
            return Ok(value);
        }
    }
}
