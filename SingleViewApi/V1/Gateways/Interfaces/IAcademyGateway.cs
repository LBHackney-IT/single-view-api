using System.Threading.Tasks;

namespace SingleViewApi.V1.Gateways.Interfaces;

public interface IAcademyGateway
{
    Task<dynamic> GetCouncilTaxAccountsByCustomerName(string firstName, string lastName);
}
