using System.Threading.Tasks;

namespace Wdpr_Groep_E.Services
{
    public interface IZmdhApi
    {
         Task GetClient();
         Task GetClientObject();
         Task PostClient();
         Task DeleteClient();
         Task PutClient();
        
        
    }


}