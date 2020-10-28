using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using TP1_ARQWEB.Areas.Identity.Data;
using TP1_ARQWEB.Models.ViewModels;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace TP1_ARQWEB.Controllers
{
    public class MapController: Controller
    {
        private readonly DBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MapController(UserManager<ApplicationUser> userManager, DBContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> IndexAsync()
        {
            var locations = await _context.Location.ToListAsync();
            var mapLocations = new List<MapLocation>();
            foreach (var l in locations)
            {
                var mapLoc = new MapLocation
                {
                    Nombre = l.Nombre,
                    Lat = l.Latitud,
                    Lng = l.Longitud,
                    Cap = l.Capacidad,
                    Conc = l.CantidadPersonasDentro
                };
                mapLocations.Add(mapLoc);
            }
            return View(mapLocations);

        }
    }
}
