using System.Collections.Generic;
using System.Threading.Tasks;
using SingleViewApi.V1.Domain;

namespace SingleViewApi.V1.Gateways;

public interface ICautionaryAlertsGateway
{
    public Task<List<CautionaryAlert>> GetCautionaryAlertsById(string id, string userToken);
}
