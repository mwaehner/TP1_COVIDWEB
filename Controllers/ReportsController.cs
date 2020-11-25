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
using Microsoft.AspNetCore.Identity.UI.Services;
using TP1_ARQWEB.Areas.Identity.Data;
using System.Security.Claims;
using TP1_ARQWEB.Helpers;
using System.Net.Mail;
using TP1_ARQWEB.Mail;
using TP1_ARQWEB.Services;

namespace TP1_ARQWEB.Controllers
{
    public class ReportsController : Controller
    {
        private readonly DBContext _context;
        private readonly UserInfoManager _userInfoManager;
        private readonly IEmailSender _emailSender;

        public ReportsController(DBContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userInfoManager = new UserInfoManager(userManager, context);
            _emailSender = emailSender;
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
            var currentUser = await _userInfoManager.FindUser(User);

            var infectionReport = await _userInfoManager.GetOpenInfectionReport(currentUser);

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
            var currentUser = await _userInfoManager.FindUser(User);

            if (currentUser.Infected)
                return RedirectToAction(nameof(Index));

            return View();
        }

        // POST: Reports/InfectionReport
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.


        private bool intersectAtLeast15Minutes(Stay stay1, Stay stay2)
        {

            DateTime TimeOfExit1 = stay1.TimeOfExit ?? Time.Now();
            DateTime TimeOfExit2 = stay2.TimeOfExit ?? Time.Now();

            if (TimeOfExit1 < stay1.TimeOfEntrance.AddMinutes(15) || TimeOfExit2 < stay2.TimeOfEntrance.AddMinutes(15)) return false;
            if (stay1.TimeOfEntrance <= stay2.TimeOfEntrance && stay2.TimeOfEntrance.AddMinutes(15) <= TimeOfExit1) return true;
            if (stay2.TimeOfEntrance <= stay1.TimeOfEntrance && stay1.TimeOfEntrance.AddMinutes(15) <= TimeOfExit2) return true;
            return false;
        }

        private DateTime minDate(DateTime date1, DateTime date2)
        {
            if (date1 < date2) return date1;
            return date2;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> InfectionReport([Bind("Id,DiagnosisDate")] InfectionReport infectionReport)
        {


            var currentUser = await _userInfoManager.FindUser(User);
            if (currentUser.Infected)
                return RedirectToAction(nameof(Index));
            infectionReport.ApplicationUserId = currentUser.Id;
            infectionReport.DischargedDate = null;

            if (infectionReport.DiagnosisDate > Time.Now())
            {
                ModelState.AddModelError("DiagnosisDate", "La fecha de diagnosis no puede ser posterior a la fecha actual.");
                return View(infectionReport);
            }

            _context.Add(infectionReport);
            await _context.SaveChangesAsync();

            currentUser.InfectionStatus = InfectionStatus.Infected;
            currentUser.TimeOfLastCondition = infectionReport.DiagnosisDate;
            await _userInfoManager.Update(currentUser);

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
                            var userAtRisk = await _userInfoManager.FindUserById(stay2.UserId);
                            DateTime newTimeOfCondition = minDate((stay.TimeOfExit ?? Time.Now()), (stay2.TimeOfExit ?? Time.Now()));
                            if (!userAtRisk.Infected && (userAtRisk.TimeOfLastCondition == null || userAtRisk.TimeOfLastCondition < newTimeOfCondition) )
                            {
                                userAtRisk.InfectionStatus = InfectionStatus.AtRisk;
                                userAtRisk.TimeOfLastCondition = newTimeOfCondition;
                                await _userInfoManager.Update(userAtRisk);
                                await _emailSender.SendEmailAsync(userAtRisk.Email,
                                    "ADVERTENCIA: Riesgo de Contagio",
                                    "Se ha registrado que usted estuvo en contacto con alguien que recientemente contrajo CoronaVirus alrededor de la fecha" + newTimeOfCondition.ToString() + ". Por favor considere realizar un Test de CoronaVirus para asegurar su salud.");


                                var newNotification = new Notification
                                {
                                    NotificationType = Notification.Type.AtRisk,
                                    UserId = userAtRisk.Id,
                                    Date = newTimeOfCondition
                                };
                                _context.Notification.Add(newNotification);
                                await _context.SaveChangesAsync();

                            }
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
            var currentUser = await _userInfoManager.FindUser(User);
            if (!currentUser.Infected)
                return RedirectToAction(nameof(Index));

            var infectionReport = await _userInfoManager.GetOpenInfectionReport(currentUser);

            if (infectionReport == null)
            {
                return NotFound();
            }

            var infectionDischarge = new InfectionDischarge
            {
                InfectionReportId = infectionReport.Id,
                DiagnosisDate = infectionReport.DiagnosisDate,
                DischargedDate = Time.Now()
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
            if (infectionDischarge.DischargedDate > Time.Now())
            {
                ModelState.AddModelError("DischargedDate", "La fecha de dada de alta no puede ser posterior a la fecha actual.");
            }
            if (!ModelState.IsValid) return View(infectionDischarge);

            infectionReport.DischargedDate = infectionDischarge.DischargedDate;

            _context.Update(infectionReport);

            await _context.SaveChangesAsync();


            var currentUser = await _userInfoManager.FindUser(User);
            currentUser.InfectionStatus = InfectionStatus.Healthy; // Agregar estado Recovered.
            currentUser.TimeOfLastCondition = null;
            await _userInfoManager.Update(currentUser);

            return RedirectToAction("Index", "Home");



        }

        // GET: Reports/NegativeTest
        [Authorize]
        public async Task<IActionResult> NegativeTest()
        {
            var currentUser = await _userInfoManager.FindUser(User);

            if (!currentUser.AtRisk)
                return RedirectToAction(nameof(Index));

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> NegativeTest([Bind("Id,TestDate")] NegativeTest negativeTest)
        {


            var currentUser = await _userInfoManager.FindUser(User);
            if (!currentUser.AtRisk) 
                return RedirectToAction(nameof(Index));
            negativeTest.ApplicationUserId = currentUser.Id;

            if (negativeTest.TestDate > Time.Now())
            {
                ModelState.AddModelError("TestDate", "La fecha de realización del test no puede ser posterior a la fecha actual.");
                return View(negativeTest);
            }
            if (negativeTest.TestDate < currentUser.TimeOfLastCondition)
            {
                ModelState.AddModelError("TestDate", "La fecha de realización del test debe ser posterior a la última vez que entró en contacto con alguien contagiado.");
                return View(negativeTest);
            }


            _context.Add(negativeTest);
            await _context.SaveChangesAsync();

            currentUser.InfectionStatus = InfectionStatus.Healthy;
            currentUser.TimeOfLastCondition = null;
            await _userInfoManager.Update(currentUser);


            return RedirectToAction(nameof(Index));
        }


    }


}

