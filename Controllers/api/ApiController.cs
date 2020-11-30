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
    // Por qué hereda de ControllerBase en vez de Controller?
    public class ApiController : ControllerBase
    {
        private readonly DBContext _context;
        private readonly ICheckService _checkService;
        private readonly IInfectionManager _infectionManager;
        public ApiController(DBContext context, ICheckService checkService, IInfectionManager infectionManager)
        {
            _context = context;
            _checkService = checkService;
            _infectionManager = infectionManager;
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

        public async Task<IActionResult> checkin(int? id)
        {
            if (id != null)
            {

                CheckResult result = await _checkService.Checkin((int)id);

                if (result.successful)
                {
                    return RedirectToAction("location", "Api", new { id });
                }

                return NotFound(JsonSerializer.Serialize(result));
            }

            CheckResult checkResult = new CheckResult { successful = false, message = "Debe proveer el id de la locacion que desea buscar" };


            return NotFound(JsonSerializer.Serialize(checkResult));

        }

        public async Task<IActionResult> checkout(int? id)
        {
            if (id != null)
            {

                CheckResult result = await _checkService.Checkout((int)id);

                if (result.successful)
                {
                    return RedirectToAction("location", "Api", new { id });
                }

                return NotFound(JsonSerializer.Serialize(result));
            }

            CheckResult checkResult = new CheckResult { successful = false, message = "Debe proveer el id de la locacion que desea buscar" };


            return NotFound(JsonSerializer.Serialize(checkResult));

        }

        public async Task<IActionResult> contagion(string inJson)
        {

            var ExternalStays = JsonSerializer.Deserialize<ListOfExStays>(inJson);

            var Stays = new List<Stay>();

            foreach (var externalStay in ExternalStays.stays)
            {
                Stays.Add(externalStay.ToLocalStay());
            }

            await _infectionManager.UpdateRiskStatusFromStays((IQueryable<Stay>)Stays);

            return Ok();

        }

    }
}
