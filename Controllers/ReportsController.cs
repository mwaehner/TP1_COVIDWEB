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
        private readonly IUserInfoManager _userInfoManager;
        private readonly IEmailSender _emailSender;
        private readonly IInfectionService _infectionManager;

        public ReportsController(DBContext context, IUserInfoManager userInfoManager, IEmailSender emailSender, IInfectionService infectionManager)
        {
            _context = context;
            _userInfoManager = userInfoManager;
            _emailSender = emailSender;
            _infectionManager = infectionManager;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> InfectionReport([Bind("Id,DiagnosisDate")] InfectionReport infectionReport)
        {

            var currentUser = await _userInfoManager.FindUser(User);

            try { await _infectionManager.NewInfectionReport(currentUser, infectionReport); }
            catch (ModelException ex)
            {
                ex.UpdateModelState(ModelState);
                return View(infectionReport);
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
            var currentUser = await _userInfoManager.FindUser(User);

            try { await _infectionManager.DischargeUser(currentUser, infectionDischarge); }
            catch (ModelException ex)
            {
                ex.UpdateModelState(ModelState);
                return View(infectionDischarge);

            }

            return RedirectToAction("Index", "Home");



        }

        // GET: Reports/NegativeTest
        [Authorize]
        public async Task<IActionResult> NegativeTest()
        {
            var currentUser = await _userInfoManager.FindUser(User);

            if (!currentUser.AtRisk)
                return RedirectToAction(nameof(Index));

            ViewBag.TimeOfLastCondition = currentUser.TimeOfLastCondition;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> NegativeTest([Bind("Id,TestDate")] NegativeTest negativeTest)
        {


            var currentUser = await _userInfoManager.FindUser(User);

            try { await _infectionManager.NewNegativeTest(currentUser, negativeTest); }
            catch (ModelException ex)
            {
                ex.UpdateModelState(ModelState);
                ViewBag.TimeOfLastCondition = currentUser.TimeOfLastCondition;
                return View(negativeTest);
            }

            return RedirectToAction(nameof(Index));
        }


    }


}

