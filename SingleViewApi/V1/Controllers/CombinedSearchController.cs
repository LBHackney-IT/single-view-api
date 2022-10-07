using System;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.Controllers;

[ApiController]
[Route("api/v1/search")]
[Produces("application/json")]
[ApiVersion("1.0")]
public class CombinedSearchController : BaseController
{
    private readonly IGetCombinedSearchResultsByNameUseCase _getCombinedSearchResultsByNameUseCase;

    public CombinedSearchController(IGetCombinedSearchResultsByNameUseCase getCombinedSearchResultsByNameUseCase)
    {
        _getCombinedSearchResultsByNameUseCase = getCombinedSearchResultsByNameUseCase;
    }

    //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-SingleViewApi/wiki/Controllers-and-Response-Objects)
    /// <summary>
    ///     ...
    /// </summary>
    /// <response code="200">...</response>
    /// <response code="400">Invalid Query Parameter.</response>
    [ProducesResponseType(typeof(SearchResponseObject), StatusCodes.Status200OK)]
    [HttpGet]
    public IActionResult SearchByName([FromQuery] string firstName, string lastName, [Optional] string dateOfBirth,
        string redisId, [FromHeader] string authorization)
    {
        try
        {
            return Ok(_getCombinedSearchResultsByNameUseCase
                .Execute(firstName, lastName, authorization, redisId, dateOfBirth).Result);
        }
        catch (Exception e)
        {
            Console.WriteLine("search error ---------------");
            Console.WriteLine(e);
            throw;
        }
    }
}
