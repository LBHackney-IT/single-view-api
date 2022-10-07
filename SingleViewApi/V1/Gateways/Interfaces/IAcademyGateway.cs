using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary.Response;

namespace SingleViewApi.V1.Gateways.Interfaces;

public interface IAcademyGateway
{
    Task<CouncilTaxSearchResponseObject> GetCouncilTaxAccountsByCustomerName(string firstName, string lastName, string userToken);

    Task<HousingBenefitsSearchResponseObject> GetHousingBenefitsAccountsByCustomerName(string firstName, string lastName, string userToken);

    Task<CouncilTaxRecordResponseObject> GetCouncilTaxAccountByAccountRef(string accountRef, string userToken);
    Task<HousingBenefitsRecordResponseObject> GetHousingBenefitsAccountByAccountRef(string accountRef, string userToken);

    Task<List<AcademyNotesResponseObject>> GetHousingBenefitsNotes(string accountRef, string userToken);
    Task<List<AcademyNotesResponseObject>> GetCouncilTaxNotes(string councilTaxId, string userToken);

}
