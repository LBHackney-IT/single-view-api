using Hackney.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.Controllers;

[ApiController]
[Route("api/v1/storeCredentials")]
[Produces("application/json")]
[ApiVersion("1.0")]
public class StoreJigsawCredentialsController : BaseController
{
    private readonly IStoreJigsawCredentialsUseCase _storeJigsawCredentialsUseCase;

    public StoreJigsawCredentialsController(IStoreJigsawCredentialsUseCase storeJigsawCredentialsUseCase)
    {
        _storeJigsawCredentialsUseCase = storeJigsawCredentialsUseCase;
    }

    //TODO: add xml comments containing information that will be included in the auto generated swagger docs (https://github.com/LBHackney-IT/lbh-SingleViewApi/wiki/Controllers-and-Response-Objects)
    /// <summary>
    ///     ...
    /// </summary>
    /// <response code="200">...</response>
    /// <response code="400">Invalid Query Parameter.</response>
    [ProducesResponseType(typeof(ResponseObject), StatusCodes.Status200OK)]
    [HttpPost]
    [LogCall(LogLevel.Information)]
    public IActionResult StoreJigsawCredentials([FromBody] string encryptedCredentials,
        [FromHeader] string authorization)
    {
        var id = _storeJigsawCredentialsUseCase.Execute(encryptedCredentials, authorization);

        if (string.IsNullOrEmpty(id)) return Unauthorized("Credentials are incorrect");

        return Ok(id);
    }
}
