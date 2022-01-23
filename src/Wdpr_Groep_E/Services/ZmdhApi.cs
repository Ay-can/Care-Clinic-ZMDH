using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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

        public async Task<int> CreateClientId()
        {
            var getClients = await GetAllClients();
            var getLastClientId = getClients.Last();
            int parseClienetId = int.Parse(getLastClientId) + 1;

            return parseClienetId;
        }

        public async Task<IEnumerable<Client>> GetClients()
        {
            IEnumerable<Client> allClients;
            var request = new HttpRequestMessage(HttpMethod.Get, Url + Key);
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            allClients = await response.Content.ReadFromJsonAsync<IEnumerable<Client>>();
            return allClients;
        }

        // public async Task<int>PostClient()
        // {
        //     var send = await HttpClient.PostAsync(Key,null);
        //     Console.WriteLine(send.Content.ReadAsStringAsync().Result);

        //     int result = int.Parse(send.Content.ReadAsStringAsync().Result);
        //     System.Console.WriteLine(result);
        //     return result;
        // }
        //  public async Task PostClientTest()
        // {

        //     var send =  await HttpClient.PostAsync(Key,null);
        //     Console.WriteLine(send.Content.ReadAsStringAsync().Result);
        // }

        public async Task PostClient(Client c)
        {
            var send = await HttpClient.PostAsJsonAsync(Key, c);
            System.Console.WriteLine(send.Content.ReadAsStringAsync().Result);
        }

        public async Task PutClient(Client c)
        {
            var send = await HttpClient.PutAsJsonAsync(Key, c);
            Console.WriteLine(send.Content.ReadAsStringAsync().Result);
        }
    }
}
