using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TP1_ARQWEB.Areas.Identity.Data;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;
using TP1_ARQWEB.Services;

namespace TP1_ARQWEB.Controllers
{
    [Authorize]
    //solamente las personas que esten autenticadas pueden ingresar al home
    public class QRController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DBContext _context;


        public QRController(UserManager<ApplicationUser> userManager, DBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        public IActionResult DecodeQR(string text)
        {


            var qrHandler = new QRHandler();
            QRData qrData;
            try
            {
                qrData = qrHandler.DecodeQRCode(text);
            } catch (Exception ex)
            {
                return RedirectToAction("QRCodeReader");
            }


            return RedirectToAction("Details","Check", new { id = Int32.Parse(qrData.location_id), serverId = Int32.Parse(qrData.server_id) });
        }

        [Authorize]
        public IActionResult QRCodeReader()
        {
            return View();
        }


    }
}
