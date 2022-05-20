using System.Threading.Tasks;
using SingleViewApi.V1.Gateways;
using SingleViewApi.V1.UseCase.Interfaces;

namespace SingleViewApi.V1.UseCase;

public class GetJigsawCustomerByIdUseCase : IGetJigsawCustomerByIdUseCase

{
    private IJigsawGateway _jigsawGateway;
    private IGetJigsawAuthTokenUseCase _getJigsawAuthTokenUseCase;

    public GetJigsawCustomerByIdUseCase(IJigsawGateway jigsawGateway, IGetJigsawAuthTokenUseCase getJigsawAuthTokenUseCase)
    {
        _jigsawGateway = jigsawGateway;
        _getJigsawAuthTokenUseCase = getJigsawAuthTokenUseCase;
    }

    public async Task<dynamic> Execute(string customerId, string redisId)
    {
        var jigsawAuthToken = await _getJigsawAuthTokenUseCase.Execute(redisId);
        return await _jigsawGateway.GetCustomerById(customerId, jigsawAuthToken);
    }

}
