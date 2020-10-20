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

            dynamic model = new ExpandoObject();
            model.location = location;
            model.idActualLocation = idActual;

            return View(model);
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

            string userIdValue = _userManager.GetUserId(User);

            

            UserAppInfo currentUser = await _context.UserAppInfo
                .FirstOrDefaultAsync(m => m.Id == userIdValue);

            bool userInLocation = currentUser.CurrentLocationId == location.Id;

            dynamic model = new ExpandoObject();
            model.location = location;
            model.userInLocation = userInLocation;


            return View(model);
        }

        // POST: Locations/Check/Out/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Out(int? Id) 
            // el parametro Id no es la Id de la locacion de la que se hará Check Out sino de la locacion
            // cuyos detalles se mostran luego del Check Out
        {
            string userIdValue = _userManager.GetUserId(User);

            UserAppInfo currentUser = await _context.UserAppInfo
                .FirstOrDefaultAsync(m => m.Id == userIdValue);

            if (currentUser.CurrentLocationId != null)
            {
                Stay currentStay = await _context.Stay
                .FirstOrDefaultAsync(m => m.Id == currentUser.CurrentStayId);

                currentUser.CurrentLocationId = null;
                currentUser.CurrentStayId = null;
                _context.Update(currentUser);

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
            string userIdValue = _userManager.GetUserId(User);

            UserAppInfo currentUser = await _context.UserAppInfo
                .FirstOrDefaultAsync(m => m.Id == userIdValue);

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
                _context.Update(currentUser);

                await _context.SaveChangesAsync();
            } else if (currentUser.CurrentLocationId != Id)
            {
                return RedirectToAction("OutBeforeIn", new { idActual = Id, idAnterior = currentUser.CurrentLocationId });
            }

            return RedirectToAction("Details", new { id = Id });

        }
    }
}
