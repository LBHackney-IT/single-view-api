using System.Threading.Tasks;
using Hackney.Shared.ContactDetail.Domain;

namespace SingleViewApi.V1.Gateways.Interfaces
{
    public interface IContactDetailsGateway
    {
        Task<ContactDetails> GetContactDetailsById(string id, string userToken);
    }
}
