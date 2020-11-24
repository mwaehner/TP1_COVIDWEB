using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP1_ARQWEB.Areas.Identity.Data;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;
using QRCoder;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace TP1_ARQWEB.Services
{
    public class QRHandler
    {
        public bool IsValid(string data)
        {
            return true;
        }
        public string EncodeLocationIdToBase64(int id)
        {
            var toEncode = new QRData()
            {
                location_id = id.ToString(),
                server_id = "2"
            };

            string stringToEncode = JsonConvert.SerializeObject(toEncode);

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(stringToEncode, QRCodeGenerator.ECCLevel.Q);
            Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }

        public QRData DecodeQRCode(string data)
        {
            return JsonConvert.DeserializeObject<QRData>(data);
        }

    }
}
