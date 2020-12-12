using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TP1_ARQWEB.Models;
using Newtonsoft.Json;
using System.Linq;

namespace TP1_ARQWEB.Services
{

    public interface IExternalPlatformService
    {
        public string GetApiBaseURI(int serverId);
        public Task<Location> GetLocation(int locationId, int serverId);
        public bool IsForeign(int server_id);
        public Task<CheckResult> ExternalCheckIn(int locationId, int serverId);
        public Task<CheckResult> ExternalCheckOut(int locationId, int serverId);
        Task notifyOtherServers(IQueryable<Stay> userStays);
    }
    public class ExternalPlatformService : IExternalPlatformService
    {

        private Dictionary<int, string> externalPlatforms;
        public ExternalPlatformService()
        {
            externalPlatforms = new Dictionary<int, string>(){
                {0, "http://yoestuveahiyea.herokuapp.com/"},
                {1, "http://52.91.22.119/api/"},
                {2, "https://localhost:5001/api/" },
                {3, "https://f47e5f8a9309aff973c00011cc95f016.m.pipedream.net/api/" }
            };
        }


        public bool IsForeign(int server_id)
        {
            return server_id != 2;
        }
        public string GetApiBaseURI(int serverId)
        {
            string baseUri = "";
            if (!externalPlatforms.TryGetValue(serverId, out baseUri))
            {
                throw new Exception("No existe un server con serverId " + serverId.ToString());

            }

            return baseUri;
        }

        public async Task<Location> GetLocation(int locationId, int serverId)
        {
            var ApiBaseURI = GetApiBaseURI(serverId);

            WebRequest request = WebRequest.Create(ApiBaseURI + "location/" + locationId);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Accept", "application/json");

            WebResponse response = await request.GetResponseAsync();
            var reader = new StreamReader(response.GetResponseStream());
            string responseContent = reader.ReadToEnd();
            var adResponse =
                JsonConvert.DeserializeObject<GeneralizedLocation>(responseContent);
            return adResponse.ToLocation(locationId);

        }


        public async Task<CheckResult> ExternalCheckIn(int locationId, int serverId)
        {
            var ApiBaseURI = GetApiBaseURI(serverId);

            WebRequest request = WebRequest.Create(ApiBaseURI + "checkin/" + locationId);
            request.Method = "POST";

            request.ContentType = "application/json";
            request.Headers.Add("Accept", "application/json");
            WebResponse response = await request.GetResponseAsync();
            var reader = new StreamReader(response.GetResponseStream());
            string responseContent = reader.ReadToEnd();
            var adResponse =
                JsonConvert.DeserializeObject<CheckResult>(responseContent);
            return adResponse;

        }
        public async Task<CheckResult> ExternalCheckOut(int locationId, int serverId)
        {
            var ApiBaseURI = GetApiBaseURI(serverId);

            WebRequest request = WebRequest.Create(ApiBaseURI + "checkout/" + locationId);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Accept", "application/json");

            WebResponse response = await request.GetResponseAsync();
            var reader = new StreamReader(response.GetResponseStream());
            string responseContent = reader.ReadToEnd();
            var adResponse =
                JsonConvert.DeserializeObject<CheckResult>(responseContent);
            return adResponse;
        }

        public async Task notifyOtherServers(IQueryable<Stay> userStays)
        {
            await otherServersDo(async (serverId, baseUrl) =>
           {

               WebRequest request = WebRequest.Create(baseUrl + "contagion/new");
               request.Method = "POST";
               request.ContentType = "application/json";
               request.Headers.Add("Accept", "application/json");

               using (var streamWriter = new StreamWriter(request.GetRequestStream()))
               {
                   string json = JsonConvert.SerializeObject(ListOfExStays.FromListOfStays(userStays));

                   streamWriter.Write(json);
               }



               try
               {

               WebResponse response = await request.GetResponseAsync();
               var reader = new StreamReader(response.GetResponseStream());
               string responseContent = reader.ReadToEnd();

               Console.WriteLine(responseContent);

               } catch(WebException ex)
               {

                   // Que hacer si se produce una excepcion.
                   Console.WriteLine(ex);
               }



               return true;


           });
        }


        private async Task otherServersDo(Func<int,string, Task<bool>> action)
        {
            foreach (var v in externalPlatforms.ToList())
            {
                if (IsForeign(v.Key))
                {
                    await action(v.Key, v.Value);
                }
            }
        }
    }
}
