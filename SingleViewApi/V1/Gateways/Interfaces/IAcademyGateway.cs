using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Gateways.Interfaces;

public interface IAcademyGateway
{
    Task<CouncilTaxSearchResponseObject> GetCouncilTaxAccountsByCustomerName(string firstName, string lastName, string userToken);

    Task<HousingBenefitsSearchResponseObject> GetHousingBenefitsAccountsByCustomerName(string firstName, string lastName, string userToken);
}
