using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Boundary;
using SingleViewApi.V1.Boundary.Response;
using SingleViewApi.V1.Helpers;

namespace SingleViewApi.V1.Gateways.Interfaces
{
    public interface IJigsawGateway
    {
        Task<AuthGatewayResponse> GetAuthToken(JigsawCredentials credentials);
        Task<List<JigsawCustomerSearchApiResponseObject>> GetCustomers(string firstName, string lastName, string bearerToken);

        Task<JigsawCustomerResponseObject> GetCustomerById(string customerId, string bearerToken);

        Task<List<JigsawNotesResponseObject>> GetCustomerNotesByCustomerId(string id, string bearerToken);

        Task<JigsawCasesResponseObject> GetCasesByCustomerId(string id, string bearerToken);

        Task<JigsawCaseOverviewResponseObject> GetCaseOverviewByCaseId(string caseId, string bearerToken);

        Task<JigsawCasePlacementInformationResponseObject> GetCaseAccommodationPlacementsByCaseId(string caseId, string bearerToken);

        Task<JigsawCaseAdditionalFactorsResponseObject> GetCaseAdditionalFactors(string caseId, string bearerToken);

        Task<JigsawCaseAdditionalFactorsResponseObject> GetCaseHealthAndWellBeing(string caseId, string bearerToken);

    }
}
