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
        [LogCall(LogLevel.Information)]
        public IActionResult GetRedis([FromQuery] string input)
        {
            Console.WriteLine("------ BOOO BOOO ------");
            string thing;
            try
            {
                thing = _redisGateway.DoTheThing(input);
                Console.WriteLine("------ GOT THE THING ------");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                thing = "no such thing";
            }

            return Ok(thing);
        }
    }
}
