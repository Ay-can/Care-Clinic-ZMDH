using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Wdpr_Groep_E.Models;

namespace Wdpr_Groep_E.Services
{
    public class ZmdhApi : IZmdhApi
    {
        private const string Url = "https://orthopedagogie-zmdh.herokuapp.com/clienten";
        private string urlParameters ="?sleutel=725630189&clientid=";
        public HttpClient HttpClient { get; set; } = new HttpClient();
        public HttpResponseMessage ResponseMessage { get; set; } = new HttpResponseMessage();

       
        public async Task DeleteClient()
        {
            throw new System.NotImplementedException();
        }

       

        public async Task<Client> GetClientObject(Client c)
        {
        HttpClient.BaseAddress = new Uri(Url);
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var httpGet = await HttpClient.GetAsync(urlParameters + c.ClientId);
        var httpResponse = await httpGet.Content.ReadAsAsync<Client>();

        return httpResponse;        

        }

       

        public async Task PostClient()
        {
            throw new System.NotImplementedException();
        }

        public async Task PutClient()
        {
            throw new System.NotImplementedException();
        }
    }
}