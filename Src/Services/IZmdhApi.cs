using System.Collections.Generic;
using System.Threading.Tasks;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Services
{
    public interface IZmdhApi
    {
         Task<Client> GetClientObject(Client c);
         Task PostClient();
         Task DeleteClient();
         Task PutClient();
        
        
    }


}