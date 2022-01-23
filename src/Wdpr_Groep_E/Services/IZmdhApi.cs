using System.Collections.Generic;
using System.Threading.Tasks;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Services
{
    public interface IZmdhApi
    {
        Task<Client> GetClientObject(string clientid);
        Task PostClient(Client c);
        Task DeleteClient(string clientid);
        Task PutClient(Client c);
        Task<IEnumerable<string>> GetAllClients();
        Task<int> CreateClientId();
        Task<IEnumerable<Client>> GetClients();
    }
}
