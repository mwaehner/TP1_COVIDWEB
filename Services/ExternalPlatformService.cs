using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using TP1_ARQWEB.Areas.Identity.Data;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;
using QRCoder;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.Drawing;

namespace TP1_ARQWEB.Services
{

    public interface IExternalPlatformService
    {
        public string GetApiBaseURI(int serverId);
        public Task<Location> GetLocation(int locationId, int serverId);
        public bool IsForeign(int server_id);
        public Task<CheckResult> ExternalCheckIn(int locationId, int serverId);
        public Task<CheckResult> ExternalCheckOut(int locationId, int serverId);

    }
    public class ExternalPlatformService : IExternalPlatformService
    {
        public bool IsForeign(int server_id)
        {
            return server_id != 2;
        }
        public string GetApiBaseURI(int serverId)
        {
            switch (serverId)
            {
                case 0:
                    return "http://yoestuveahiyea.herokuapp.com/";
                case 1:
                    return "http://52.91.22.119/api/";
                case 2:
                    return "https://localhost:5001/api/";
                default:
                    throw new Exception("Platform doesn't exist");
            }
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


    }
}
