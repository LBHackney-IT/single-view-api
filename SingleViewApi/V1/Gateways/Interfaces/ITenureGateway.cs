using System;
using System.Threading.Tasks;
using Hackney.Shared.Tenure.Boundary.Response;

namespace SingleViewApi.V1.Gateways;

public interface ITenureGateway
{
    Task<TenureResponseObject> GetTenureInformation(Guid id, string userToken);
}
