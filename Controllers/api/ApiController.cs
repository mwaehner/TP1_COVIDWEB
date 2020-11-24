using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;
using TP1_ARQWEB.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TP1_ARQWEB.Controllers.api
{
    public class ApiController : ControllerBase
    {
        private readonly DBContext _context;
        public ApiController(DBContext context)
        {
            _context = context;
        }

        // GET: api/location/{id}
        public async Task<IActionResult> location(int id)
        {
            var l = await _context.Location
                .FirstOrDefaultAsync(m => m.Id == id);

            if (l == null)
            {
                return NotFound();
            }
            var mapLoc = new MapLocation
            {
                Nombre = l.Nombre,
                Lat = l.Latitud,
                Lng = l.Longitud,
                Cap = l.Capacidad,
                Conc = l.CantidadPersonasDentro

            };
            var jsonString = JsonSerializer.Serialize(mapLoc);

            return Ok(jsonString);
        }

    }
}
