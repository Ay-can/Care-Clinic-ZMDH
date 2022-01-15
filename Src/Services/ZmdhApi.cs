using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Wdpr_Groep_E.Models;
namespace Wdpr_Groep_E.Services
{
    public class ZmdhApi : IZmdhApi
    {
        private const string Url = "https://orthopedagogie-zmdh.herokuapp.com/clienten";
        private const string Key = "?sleutel=725630189";
        private string urlParameters ="&clientid=";
        public HttpClient HttpClient { get; set; } = new HttpClient();
        public HttpResponseMessage ResponseMessage { get; set; } = new HttpResponseMessage();

        public ZmdhApi()
        {
            HttpClient.BaseAddress = new Uri(Url);
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
       
        public async Task DeleteClient(string clientid)
        {
            
            await HttpClient.DeleteAsync(Key + urlParameters + clientid);

        }

       

        public async Task<Client> GetClientObject(string clientid)
        {
        var httpGet = await HttpClient.GetAsync(Key + urlParameters + clientid);
        var httpResponse = await httpGet.Content.ReadAsAsync<Client>();

        return httpResponse;        

        }
        //Op dit moment werkt de post alleen als je een id erbij geeft, anders niet.. dit moet je fixen(met postman lukt het wel zonder id?)
        public async Task PostClient(Client c)
        {
        await HttpClient.PostAsJsonAsync(Key,c);
        }

        public async Task PutClient(Client c )
        {
            await HttpClient.PutAsJsonAsync<Client>(Key + urlParameters + c.clientid,c);
        }
    }
}