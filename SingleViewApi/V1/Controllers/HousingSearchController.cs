using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.Controllers
{
    [ApiController]
    //TODO: Rename to match the APIs endpoint
    [Route("api/v1/search")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    //TODO: rename class to match the API name
    public class HousingSearchController : BaseController
    {
        private readonly IGetSearchResultsBySearchTextUseCase _getSearchResultsBySearchTextUseCase;
        public HousingSearchController(IGetSearchResultsBySearchTextUseCase getSearchResultsBySearchTextUseCase)
        {
            _getSearchResultsBySearchTextUseCase = getSearchResultsBySearchTextUseCase;
        }

        //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-SingleViewApi/wiki/Controllers-and-Response-Objects)
        /// <summary>
        /// ...
        /// </summary>
        /// <response code="200">...</response>
        /// <response code="400">Invalid Query Parameter.</response>
        [ProducesResponseType(typeof(ResponseObjectList), StatusCodes.Status200OK)]
        [HttpGet]
        [LogCall(LogLevel.Information)]
        public IActionResult SearchBySearchText([FromQuery] string searchText, int page, string userToken)
        {
            return Ok(_getSearchResultsBySearchTextUseCase.Execute(searchText,page, userToken).Result);
        }
    }
}
