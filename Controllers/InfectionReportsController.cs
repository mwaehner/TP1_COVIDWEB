using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;
using Microsoft.AspNetCore.Identity;
using TP1_ARQWEB.Areas.Identity.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TP1_ARQWEB.Helpers;

namespace TP1_ARQWEB.Controllers
{
    public class InfectionReportsController : Controller
    {
        private readonly MvcLocationContext _context;

        public InfectionReportsController(MvcLocationContext context)
        {
            _context = context;
        }

        // GET: InfectionReports/Details/
        public async Task<IActionResult> Index()
        {

            var userIdClaim = UserHelper.getUserId(this);

            if (userIdClaim != null)
            {

                var infectionReport = await _context.InfectionReport
                    .FirstOrDefaultAsync(m => m.ApplicationUserId == userIdClaim);
                if (infectionReport == null)
                {
                    return View("Create");
                }

                return View("Details", infectionReport);
            }

            return View("Create");

        }

        [Authorize]
        // GET: InfectionReports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InfectionReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,DiagnosisDate,DischargedDate")] InfectionReport infectionReport)
        {

            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                // the principal identity is a claims identity.
                // now we need to find the NameIdentifier claim
                var userIdClaim = claimsIdentity.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null)
                {
                    var userIdValue = userIdClaim.Value;
                    infectionReport.ApplicationUserId = userIdValue;
                    _context.Add(infectionReport);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            return NotFound();
        }

        [Authorize]
        public async Task<IActionResult> Discharge(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var infectionReport = await _context.InfectionReport.FindAsync(id);
            if (infectionReport == null)
            {
                return NotFound();
            }

            var infectionDischarge = new InfectionDischarge
            {
                InfectionReportId = infectionReport.Id,
                DiagnosisDate = infectionReport.DiagnosisDate
            };


            return View("Edit", infectionDischarge);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Discharge(InfectionDischarge infectionDischarge)
        {


            if (ModelState.IsValid)
            {

                var infectionReport = await _context.InfectionReport.FindAsync(infectionDischarge.InfectionReportId);

                if (infectionReport == null)
                {
                    return NotFound();
                }




                infectionReport.DischargedDate = infectionDischarge.DischargedDate;

                _context.Update(infectionReport);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");



            }
            return View("Edit", infectionDischarge);
        }


    }
}
