using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;
using Microsoft.AspNetCore.Identity;
using TP1_ARQWEB.Areas.Identity.Data;

namespace TP1_ARQWEB.Controllers
{
    [Authorize]
    public class CheckController : Controller
    {

        private readonly DBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckController(UserManager<ApplicationUser> userManager, DBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Check/OutBeforeIn/5
        [Authorize]
        public async Task<IActionResult> OutBeforeIn(int? idActual, int? idAnterior)
        {
            if (idActual == null || idAnterior == null)
            {
                return NotFound();
            }

            var location = await _context.Location
                .FirstOrDefaultAsync(m => m.Id == idAnterior);
            if (location == null)
            {
                return NotFound();
            }

            ViewData["idActualLocation"] = idActual;

            return View(location);
        }

        // GET: Check/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Location
                .FirstOrDefaultAsync(m => m.Id == id);
            if (location == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            bool userInLocation = currentUser.CurrentLocationId == location.Id;

            ViewData["userInLocation"] = userInLocation;

            return View(location);
        }

        // POST: Locations/Check/Out/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Out(int? Id) 
            // el parametro Id no es la Id de la locacion de la que se hará Check Out sino de la locacion
            // cuyos detalles se mostran luego del Check Out
        {

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser.CurrentLocationId != null)
            {
                Stay currentStay = await _context.Stay
                .FirstOrDefaultAsync(m => m.Id == currentUser.CurrentStayId);

                currentUser.CurrentLocationId = null;
                currentUser.CurrentStayId = null;
                await _userManager.UpdateAsync(currentUser);

                currentStay.TimeOfExit = DateTime.Now;
                _context.Update(currentStay);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = Id });

        }

        // POST: Locations/Check/In/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> In(int? Id)
        {
          
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser.CurrentLocationId == null)
            {
               
                Stay newStay = new Stay
                {
                    UserId = currentUser.Id,
                    LocationId = (int)Id,
                    TimeOfEntrance = DateTime.Now,
                    TimeOfExit = null
                };
                _context.Add(newStay);
                await _context.SaveChangesAsync();

                currentUser.CurrentLocationId = Id;
                currentUser.CurrentStayId = newStay.Id;
                await _userManager.UpdateAsync(currentUser);

            } else if (currentUser.CurrentLocationId != Id)
            {
                return RedirectToAction("OutBeforeIn", new { idActual = Id, idAnterior = currentUser.CurrentLocationId });
            }

            return RedirectToAction("Details", new { id = Id });

        }
    }
}
