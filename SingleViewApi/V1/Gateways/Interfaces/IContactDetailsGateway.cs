using System.Collections.Generic;
using System.Threading.Tasks;
using Hackney.Shared.ContactDetail.Boundary.Response;
using Hackney.Shared.ContactDetail.Domain;

namespace SingleViewApi.V1.Gateways.Interfaces
{
    public interface IContactDetailsGateway
    {
        Task<List<ContactDetails>> GetContactDetailsById(string id, string userToken);
    }
}
