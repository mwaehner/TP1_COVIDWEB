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
using QRCoder;
using TP1_ARQWEB.Services;

namespace TP1_ARQWEB.Controllers
{
    public class LocationsController : Controller
    {
        private readonly DBContext _context;
        private readonly IUserInfoManager _userInfoManager;
        private readonly ILocationService _locationService;
        private readonly UserManager<ApplicationUser> _userManager;


        public LocationsController(UserManager<ApplicationUser> userManager, DBContext context, IUserInfoManager userInfoManager, ILocationService locationService)
        {
            _userManager = userManager;
            _userInfoManager = userInfoManager;
            _context = context;
            _locationService = locationService;
        }

        private ApplicationUser currentUser;
        private Location location;

        private async Task UpdateUserAndLocation(int? idLocation)
        {
            currentUser = await _userInfoManager.FindUser(User);
            location = await _locationService.GetLocationById(idLocation);
            _locationService.AssertOwnership(location, currentUser);
        }

        // GET: Locations
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userInfoManager.FindUser(User);
            return View(_locationService.GetLocationsForUser(currentUser));
        }

        // GET: Locations/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {

            try { await UpdateUserAndLocation(id); }
            catch { return NotFound(); }
            

            return View(location);
        }

        // GET: Locations/QRCode/5
        [Authorize]
        public async Task<IActionResult> QRCode(int? id)
        {
            try { await UpdateUserAndLocation(id); }
            catch { return NotFound(); }

            var qrCodeImageAsBase64 = _locationService.GetQrCode(location);

            var model = new QRCodeViewModel()
            {
                QREncodedBase64 = qrCodeImageAsBase64,
                location = location
            };

            return View(model);
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
        public async Task<IActionResult> Create([Bind("Id,Nombre,IdPropietario,Capacidad,Latitud,Longitud,AperturaHora,AperturaMinuto,CierreHora,CierreMinuto")] Location location)
        {

            var currentUser = await _userInfoManager.FindUser(User);

            try { await _locationService.CreateNewLocation(location, currentUser); }
            catch ( ModelException ex )
            {
                ex.UpdateModelState(ModelState);
                return View(location);
            }

            return RedirectToAction(nameof(Index));

        }

        // GET: Locations/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            try { await UpdateUserAndLocation(id); }
            catch { return NotFound(); }

            var model = location; 
            
            return View(model);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Nombre,Capacidad,Latitud,Longitud,AperturaHora,AperturaMinuto,CierreHora,CierreMinuto")] Location model)
        {

            try { await UpdateUserAndLocation(id); }
            catch { return NotFound(); }

            try { await _locationService.EditLocation(location, model); }
            catch (ModelException ex)
            {
                ex.UpdateModelState(ModelState);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Locations/Image/5
        [Authorize]
        public async Task<IActionResult> Image(int? id)
        {
            try { await UpdateUserAndLocation(id); }
            catch { return NotFound(); }

            ImageViewModel model = new ImageViewModel
            {
                CurrentLocation = location
            };

            return View(model);
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Image(int? id, IFormFile file)
        {
            try { await UpdateUserAndLocation(id); }
            catch { return NotFound(); }



            try { await _locationService.UpdateImage(location, file); }
            catch (ModelException ex)
            {
                ex.UpdateModelState(ModelState);
                ImageViewModel model = new ImageViewModel
                {
                    CurrentLocation = location
                };

                if (file != null)
                {
                    model.ImageName = file.Name;
                    model.ImageSize = file.Length;
                }

                return View(model);
            }

            return RedirectToAction(nameof(Image));
        }


        

        // GET: Locations/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            try { await UpdateUserAndLocation(id); }
            catch { return NotFound(); }

            return View(location);
            
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try { await UpdateUserAndLocation(id); }
            catch { return NotFound(); }

            await _locationService.RemoveLocation(location);

            return RedirectToAction(nameof(Index));
        }

        

        // POST: Locations/DeleteImage/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try { await UpdateUserAndLocation(id); }
            catch { return NotFound(); }

            await _locationService.RemoveImage(location);

            return RedirectToAction(nameof(Image), new {id = id });
        }
    }
}

