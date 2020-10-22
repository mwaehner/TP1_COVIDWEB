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
using TP1_ARQWEB.Helpers;

namespace TP1_ARQWEB.Controllers
{
    public class ReportsController : Controller
    {
        private readonly DBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsController(DBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [Authorize]
        public async Task<IActionResult> Details(int id)

        {

            var userIdClaim = UserHelper.getUserId(this);

            if (userIdClaim == null) return NotFound();

            var infectionReport = await _context.InfectionReport
                    .FirstOrDefaultAsync(m => m.ApplicationUserId == userIdClaim && m.Id == id);


            return View(infectionReport);

        }

        // GET: Reports/
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var infectionReport = await _context.InfectionReport
                    .FirstOrDefaultAsync(m => m.ApplicationUserId == currentUser.Id && m.DischargedDate == null);

            var DiagnosisDate = infectionReport?.DiagnosisDate;

            ReportsIndexViewModel model = new ReportsIndexViewModel
            {
                UserAtRisk = currentUser.AtRisk,
                UserInfected = currentUser.Infected,
                DiagnosisDate = DiagnosisDate
            };

            return View(model);
        }


        [Authorize]
        // GET: Reports/InfectionReport
        public async Task<IActionResult> InfectionReport()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser.Infected)
                return RedirectToAction(nameof(Index));

            return View();
        }

        // POST: Reports/InfectionReport
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.


        private bool intersectAtLeast15Minutes(Stay stay1, Stay stay2)
        {

            DateTime TimeOfExit1 = stay1.TimeOfExit ?? DateTime.Now;
            DateTime TimeOfExit2 = stay2.TimeOfExit ?? DateTime.Now;

            if (TimeOfExit1 < stay1.TimeOfEntrance.AddMinutes(15) || TimeOfExit2 < stay2.TimeOfEntrance.AddMinutes(15)) return false;
            if (stay1.TimeOfEntrance <= stay2.TimeOfEntrance && stay2.TimeOfEntrance.AddMinutes(15) <= TimeOfExit1) return true;
            if (stay2.TimeOfEntrance <= stay1.TimeOfEntrance && stay1.TimeOfEntrance.AddMinutes(15) <= TimeOfExit2) return true;
            return false;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> InfectionReport([Bind("Id,DiagnosisDate")] InfectionReport infectionReport)
        {


            var currentUser = await _userManager.GetUserAsync(User);
            infectionReport.ApplicationUserId = currentUser.Id;
            _context.Add(infectionReport);
            await _context.SaveChangesAsync();

            currentUser.Infected = true;
            await _userManager.UpdateAsync(currentUser);

            var stays = await _context.Stay.ToListAsync();
            List<Stay> recentUserLocations = new List<Stay>();

            foreach(var stay in stays)
            {
                if(stay.UserId == currentUser.Id && stay.TimeOfEntrance.AddDays(14)>=infectionReport.DiagnosisDate)
                {
                    foreach (var stay2 in stays)
                    {
                        if(stay2.UserId != stay.UserId && stay2.LocationId == stay.LocationId && intersectAtLeast15Minutes(stay, stay2))
                        {
                            var userAtRisk = await _userManager.FindByIdAsync(stay2.UserId);
                            userAtRisk.AtRisk = true;
                            await _userManager.UpdateAsync(currentUser);
                        }
                    }
                }
            }


            return RedirectToAction(nameof(Index));
        }

        // GET: Reports/Discharge
        [Authorize]
        public async Task<IActionResult> Discharge()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (!currentUser.Infected)
                return RedirectToAction(nameof(Index));

            var infectionReport = await _context.InfectionReport
                    .FirstOrDefaultAsync(m => m.ApplicationUserId == currentUser.Id && m.DischargedDate == null);

            if (infectionReport == null)
            {
                return NotFound();
            }

            var infectionDischarge = new InfectionDischarge
            {
                InfectionReportId = infectionReport.Id,
                DiagnosisDate = infectionReport.DiagnosisDate,
                DischargedDate = DateTime.Now
            };


            return View(infectionDischarge);
        }

        // POST: Reports/Discharge
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Discharge(InfectionDischarge infectionDischarge)
        {


            var infectionReport = await _context.InfectionReport.FindAsync(infectionDischarge.InfectionReportId);

            if (infectionReport == null)
            {
                return NotFound();
            }

            if (infectionReport.DiagnosisDate >= infectionDischarge.DischargedDate)
            {
                ModelState.AddModelError("DischargedDate", "La fecha de alta debe ser posterior a la de diagnostico");
            }
            if (!ModelState.IsValid) return View(infectionDischarge);

            infectionReport.DischargedDate = infectionDischarge.DischargedDate;

            _context.Update(infectionReport);

            await _context.SaveChangesAsync();


            var currentUser = await _userManager.GetUserAsync(User);
            currentUser.Infected = false;
            currentUser.AtRisk = false;
            await _userManager.UpdateAsync(currentUser);

            return RedirectToAction("Index", "Home");



        }

        // GET: Reports/NegativeTest
        [Authorize]
        public async Task<IActionResult> NegativeTest()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (!currentUser.AtRisk)
                return RedirectToAction(nameof(Index));

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> NegativeTest([Bind("Id,TestDate")] NegativeTest negativeTest)
        {


            var currentUser = await _userManager.GetUserAsync(User);
            negativeTest.ApplicationUserId = currentUser.Id;
            _context.Add(negativeTest);
            await _context.SaveChangesAsync();

            currentUser.AtRisk = false;
            await _userManager.UpdateAsync(currentUser);


            return RedirectToAction(nameof(Index));
        }


    }


}

