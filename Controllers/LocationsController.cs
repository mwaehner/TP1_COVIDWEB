using System;
using System.Collections.Generic;
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
using Microsoft.AspNetCore.Http;

namespace TP1_ARQWEB.Controllers
{
    public class LocationsController : Controller
    {
        private readonly DBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LocationsController(UserManager<ApplicationUser> userManager, DBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Locations
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Location.ToListAsync());
        }

        // GET: Locations/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var location = await _context.Location
                .FirstOrDefaultAsync(m => m.Id == id);
            string userIdValue = _userManager.GetUserId(User);
            if (location == null || String.IsNullOrWhiteSpace(userIdValue) || userIdValue != location.IdPropietario)
            {
                return NotFound();
            }
            return View(location);
        }

        // GET: Locations/QRCode/5
        [Authorize]
        public async Task<IActionResult> QRCode(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Location
                .FirstOrDefaultAsync(m => m.Id == id);
            string userIdValue = _userManager.GetUserId(User);
            if (location == null || String.IsNullOrWhiteSpace(userIdValue) || userIdValue != location.IdPropietario)
            {
                return NotFound();
            }
            return View(location);
        }

        // GET: Locations/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Nombre,IdPropietario,Capacidad,Latitud,Longitud")] Location location)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                string userIdValue = _userManager.GetUserId(User);
                if (!String.IsNullOrWhiteSpace(userIdValue))
                {
                    location.IdPropietario = userIdValue;
                    _context.Add(location);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }                
            }
            return View(location);
        }

        // GET: Locations/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            

            var location = await _context.Location.FindAsync(id);
            string userIdValue = _userManager.GetUserId(User);
            if (location == null || String.IsNullOrWhiteSpace(userIdValue) || userIdValue != location.IdPropietario)
            {
                return NotFound();
            }
            
            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,IdPropietario,Capacidad,Latitud,Longitud")] Location location)
        {
            if (id != location.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    string userIdValue = _userManager.GetUserId(User);
                    if (!String.IsNullOrWhiteSpace(userIdValue))
                    {
                        //location.IdPropietario = userIdValue;
                        _context.Update(location);
                        await _context.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // GET: Locations/Image/5
        [Authorize]
        public async Task<IActionResult> Image(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Location.FindAsync(id);
            string userIdValue = _userManager.GetUserId(User);
            if (location == null || String.IsNullOrWhiteSpace(userIdValue) || userIdValue != location.IdPropietario)
            {
                return NotFound();
            }

            return View(location);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Image(int? id, IFormFile file)
        {
            if (id == null) return NotFound();

            if (file != null && file.Length > 0)
            {
                var location = await _context.Location.FindAsync(id);
                location.Image = new byte[file.Length];

                file.OpenReadStream().Read(location.Image, 0, (int)file.Length);
                _context.Update(location);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Image));
        }


        

        // GET: Locations/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Location
                .FirstOrDefaultAsync(m => m.Id == id);
            string userIdValue = _userManager.GetUserId(User);
            if (location == null || String.IsNullOrWhiteSpace(userIdValue) || userIdValue != location.IdPropietario)
            {
                return NotFound();
            }
            return View(location);
            
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var location = await _context.Location.FindAsync(id);
            _context.Location.Remove(location);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationExists(int id)
        {
            return _context.Location.Any(e => e.Id == id);
        }
    }
}
