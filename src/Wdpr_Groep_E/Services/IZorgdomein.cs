using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Services
{
    public interface IZorgdomein
    {
        RSAParameters GetRSAParameters();
        string Base64(string message);
        string KeyHeader();
        Task<Referral> GetReferralObject(string birthDate, string bsn);
        Task<IEnumerable<ReferralOverview>> GetAllReferrals();
    }
}
