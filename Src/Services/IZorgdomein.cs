using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Wdpr_Groep_E.Services
{
    public interface IZorgdomein
    {
        RSAParameters GetRSAParameters();

        string Base64(string message);
        string KeyHeader();
        Task<Referral> GetReferralObject(string birthDate, string bsn);


    }





}