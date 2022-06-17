using System;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.UseCase.Interfaces
{
    public interface IGetCustomerByIdUseCase
    {
        MergedCustomerResponseObject Execute(Guid customerId, string userToken, string redisId);
    }
}
