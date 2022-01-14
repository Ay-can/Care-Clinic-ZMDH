using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Wdpr_Groep_E.Services
{
    public class ZmdhApi : IZmdhApi
    {
        private const string Url = "https://orthopedagogie-zmdh.herokuapp.com/clienten?";
        private string urlParameters ="?sleutel=725630189&";
        public HttpClient Client { get; set; } = new HttpClient();
        public HttpResponseMessage ClientResponse  { get; set; }
        public Task DeleteClient()
        {
            throw new System.NotImplementedException();
        }

        public Task GetClient()
        {
            throw new System.NotImplementedException();
        }

        public Task GetClientObject()
        {
            Client.BaseAddress = new Uri(Url);
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            


        }

        public Task PostClient()
        {
            throw new System.NotImplementedException();
        }

        public Task PutClient()
        {
            throw new System.NotImplementedException();
        }
    }
}