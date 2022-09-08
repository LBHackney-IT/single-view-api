using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Request;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Gateways.Interfaces;

public interface ISharedPlanGateway
{
    Task<SharedPlanResponseObject> GetSharedPlans(GetSharedPlanRequest getSharedPlanRequest , string userToken);
}
