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

namespace TP1_ARQWEB.Controllers
{
    [Authorize]
    //solamente las personas que esten autenticadas pueden ingresar al home
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DBContext _context;


        public HomeController(UserManager<ApplicationUser> userManager, ILogger<HomeController> logger, DBContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var notificationsForUser = from notification in _context.Notification
                                       where notification.UserId == user.Id
                                       orderby notification.Date descending
                                       select notification;

            return View(notificationsForUser);
        }

        // POST: ClearNotification/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ClearNotification(int? Id)
        {
            Notification notification = await _context.Notification.FindAsync(Id);
            if (notification != null)
            {
                _context.Notification.Remove(notification);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
