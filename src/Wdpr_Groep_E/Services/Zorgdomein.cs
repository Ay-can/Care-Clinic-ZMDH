using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Services
{
    public class Zorgdomein : IZorgdomein
    {
        private string Url = "https://zorgdomeinhhs.azurewebsites.net/";
        private string urlParameters = "referral";
        private string privateKey =
            @"-----BEGIN RSA PRIVATE KEY-----
        MIICXQIBAAKBgQC707uhyk2V+EXEhFNkX96UXE10E6HaK66laLdqLuy+ei8++XAM
        GF1+Mu+mMpAqQKqxqVLsiqfW6ZZ6tat5rsWCxGou72Nu/R3j1BvRoKi/mbffnadW
        jokhi1UaEO8mn24gBJrWeb5sYcpBJe9besH0wqHCsvF7oFEXweKktInSsQIDAQAB
        AoGAEtUQ7BDnpJDHFgQahGbkX0W98lSBluloUmdkdH4N+K8xi4PhCyVqQlwDEUvi
        jon7U2Lh0Ju6Zl73WohakBHI9b6YtppZfY4WFxj75R12zTOFsr3o3jHT2c9DrShM
        nwwHG/DrV36r8Ak5Olao873DLSzq8VFzmm1gKxLU7fs40sECQQDceAAevXTlT3D4
        hz+AFHmMf9oaJkKhua6iLwCRBJKiWjtuGVWVReQSLLEY76oS1BQWs/Ore4spOFkg
        zbQlB5sZAkEA2hkEbNTjKZ/iqtfFdhj7Mttxy9PSpDpT5RQBlGrbVhfUOVf+9qXa
        bjAkzQRGR5itM/gpoe+65beaKd3v0H//WQJBAKcMQcMI+F6bj9Sv3bx1Rxfe8+nm
        XYxeveRjSsGWvmhHiEpG5eLh/wqKVHG5fpsfmE0PcqzXQj0sVdQWKM358lkCQAaN
        c92VM9H/VL8PRoZ6z6lCgJPAJHb8raKXTEjaQQbAJocmhqmAaCShW6mxNC9YffOu
        xAlye2oFbyNM4LSh89kCQQDWN3dPjtSx397v/WjIqO1LCjj5zT2uSLO5Wx4sJp/4
        Xkr7eT6cnWJp60Xlca65DmOdPYOr6X4eEED6eoF00q3T
        -----END RSA PRIVATE KEY-----";
        public HttpClient HttpClient { get; set; } = new HttpClient();
        private readonly IHttpClientFactory _clientFactory;

        public Zorgdomein(IHttpClientFactory clientFactory)
        {
            HttpClient.BaseAddress = new Uri(Url);
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _clientFactory = clientFactory;
        }

        public string Base64(string message)
        {
            byte[] baseResult;
            using (var createRsa = RSA.Create())
            {
                createRsa.ImportParameters(GetRSAParameters());
                baseResult = createRsa.SignData(Encoding.UTF8.GetBytes(message), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
            return Convert.ToBase64String(baseResult);
        }

        public async Task<Referral> GetReferralObject(string birthDate, string bsn)
        {
            Referral r;
            var request = new HttpRequestMessage(HttpMethod.Get, Url + urlParameters + "/" + birthDate + "/" + bsn);
            request.Headers.Add("key", KeyHeader());
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            r = await response.Content.ReadFromJsonAsync<Referral>();
            return r;

        }

        public async Task<IEnumerable<ReferralOverview>> GetAllReferrals()
        {
            IEnumerable<ReferralOverview> allReferrals;
            var request = new HttpRequestMessage(HttpMethod.Get, Url + urlParameters);
            request.Headers.Add("key", KeyHeader());
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            allReferrals = await response.Content.ReadFromJsonAsync<IEnumerable<ReferralOverview>>();
            return allReferrals;
        }

        public RSAParameters GetRSAParameters()
        {
            var createRsa = RSA.Create();
            createRsa.ImportFromPem(privateKey.ToCharArray());
            return createRsa.ExportParameters(true);
        }

        public string KeyHeader()
        {
            string GetUTCDateTime = DateTime.UtcNow.ToString("dd MM yyyy HH mm ss");
            string Base64Signature = Base64(GetUTCDateTime);
            return GetUTCDateTime + "|" + Base64Signature;
        }
    }
}
