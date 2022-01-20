using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Wdpr_Groep_E.Models;
namespace Wdpr_Groep_E.Services
{
    public class ZmdhApi : IZmdhApi
    {
        private const string Url = "https://orthopedagogie-zmdh.herokuapp.com/clienten";
        private const string Key = "?sleutel=725630189";
        private string urlParameters = "&clientid=";
        public HttpClient HttpClient { get; set; } = new HttpClient();
        public HttpResponseMessage ResponseMessage { get; set; } = new HttpResponseMessage();

        private readonly IHttpClientFactory _clientFactory;

        public ZmdhApi(IHttpClientFactory factory)
        {
            HttpClient.BaseAddress = new Uri(Url);
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _clientFactory = factory;
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

        public async Task<IEnumerable<string>> GetAllClients()
        {
            var httpGet = await HttpClient.GetAsync(Key);
            var httpResponse = await httpGet.Content.ReadAsAsync<IEnumerable<string>>();
            return httpResponse;
        }

        public async Task<string> CreateClientId()
        {
            var getClients = await GetAllClients();
            var getLastClientId = getClients.Last();
            int parseClienetId = int.Parse(getLastClientId) + 1;

            return parseClienetId.ToString();
        }
        public async Task PostClient(Client c)
        {
            await HttpClient.PostAsJsonAsync<Client>(Key, c);
        }

        public async Task PutClient(Client c)
        {
            await HttpClient.PutAsJsonAsync<Client>(Key + urlParameters + c.clientid, c);
        }

        // public async Task GetTest()
        // {
        //     Client c;
        //     var request = new HttpRequestMessage(HttpMethod.Get,"https://orthopedagogie-zmdh.herokuapp.com/clienten?sleutel=725630189");
        //     var client = _clientFactory.CreateClient();

        //     HttpResponseMessage response = await client.SendAsync(request);
        //     if(response.IsSuccessStatusCode)
        //     {
        //         c = await response.Content.ReadFromJsonAsync<Client>();
        //     }
        //     else
        //     {
        //         Console.WriteLine("Het ging fout");
        //         System.Console.WriteLine(response.ReasonPhrase);
        //     }
        // }

    }
}