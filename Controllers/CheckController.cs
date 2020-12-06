﻿using System;
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
using TP1_ARQWEB.Services;

namespace TP1_ARQWEB.Controllers
{
    [Authorize]
    public class CheckController : Controller
    {

        private readonly DBContext _context;
        private readonly IUserInfoManager _userInfoManager;
        private readonly ILocationService _locationService;

        public CheckController(IUserInfoManager userInfoManager, DBContext context, ILocationService locationService)
        {
            _context = context;
            _userInfoManager = userInfoManager;
            _locationService = locationService;
        }
        

        // GET: Check/OutBeforeIn/5
        [Authorize]
        public async Task<IActionResult> OutBeforeIn(int? idActual, int? idAnterior, int? serverIdActual)
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
            ViewData["serverIdActualLocation"] = serverIdActual;

            return View(location);
        }

        // GET: Check/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id, int? serverId)
        {
            if (id == null)
            {
                return NotFound();
            }
            Location location;
            try { location = await _locationService.GetLocationById(id, serverId); }
            catch { return NotFound(); }


            var currentUser = await _userInfoManager.FindUser(User);

            CheckDetailsViewModel model = new CheckDetailsViewModel
            {
                location = location,
                UserAtRisk = currentUser.AtRisk,
                UserInfected = currentUser.Infected,
                LocationFull = location.CantidadPersonasDentro >= location.Capacidad,
                UserInLocation = currentUser.CurrentLocationId == location.Id,
                serverId = (int)serverId
            };

            return View(model);
        }

        // POST: Locations/Check/Out/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Out(int? Id, int? serverId) 
            // el parametro Id no es la Id de la locacion de la que se hará Check Out sino de la locacion
            // cuyos detalles se muestran luego del Check Out
        {

            var currentUser = await _userInfoManager.FindUser(User);

            if (currentUser.CurrentLocationId != null)
            {
                try
                {
                
                var location = await _context.Location.FindAsync(currentUser.CurrentLocationId);
                location.CantidadPersonasDentro--;

                _context.Update(location);


                Stay currentStay = await _context.Stay
                .FirstOrDefaultAsync(m => m.Id == currentUser.CurrentStayId);


                currentUser.CurrentLocationId = null;
                currentUser.CurrentStayId = null;
                await _userInfoManager.Update(currentUser);

                currentStay.TimeOfExit = Time.Now();
                _context.Update(currentStay);

                await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException) { }

            }

            return RedirectToAction("Details", new { id = Id, serverId = serverId });

        }

        // POST: Locations/Check/In/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> In(int? Id, int? serverId)
        {
          
            var currentUser = await _userInfoManager.FindUser(User);
            Location location;
            try { location = await _context.Location.FindAsync(Id); }
            catch { return NotFound(); }


            if (!currentUser.Infected && location.CantidadPersonasDentro < location.Capacidad && location.Abierto())
            {

                if (currentUser.CurrentLocationId == null)
                {

                    try
                    {

                        location.CantidadPersonasDentro++;

                        _context.Update(location);
                        await _context.SaveChangesAsync();

                        Stay newStay = new Stay
                        {
                            UserId = currentUser.Id,
                            LocationId = (int)Id,
                            TimeOfEntrance = Time.Now(),
                            TimeOfExit = null
                        };
                        _context.Add(newStay);
                        await _context.SaveChangesAsync();

                        currentUser.CurrentLocationId = Id;
                        currentUser.CurrentStayId = newStay.Id;
                        await _userInfoManager.Update(currentUser);
                    } catch (DbUpdateConcurrencyException) { }

                    
      

                }
                else if (currentUser.CurrentLocationId != Id)
                {
                    return RedirectToAction("OutBeforeIn", new { idActual = Id, idAnterior = currentUser.CurrentLocationId, serverIdActual = serverId });
                }
            }

            return RedirectToAction("Details", new { id = Id , serverId = serverId});

        }
    }
}
