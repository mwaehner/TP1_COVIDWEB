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
using TP1_ARQWEB.Services;

namespace TP1_ARQWEB.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly DBContext _context;
        private readonly UserInfoManager _userInfoManager;

        public StatisticsController(UserManager<ApplicationUser> userManager, DBContext context)
        {
            _userInfoManager = new UserInfoManager(userManager, context);
            _context = context;
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {

            int locationsAmount = _context.Location.Count();
            int checkinsAmount = _context.Stay.Count();
            int usersAmount = _userInfoManager.Users().Count();
            int infectionReportsAmount = _context.InfectionReport.Count();
            int negativeTestsAmount = _context.NegativeTest.Count();

            StatisticsViewModel statisticsModel = new StatisticsViewModel(usersAmount,
                locationsAmount,
                checkinsAmount,
                infectionReportsAmount,
                negativeTestsAmount);

            return View(statisticsModel);

        }
        public InfectionStatus calculateInfectionStatus (ApplicationUser user)
        {
            if (user.AtRisk) return InfectionStatus.AtRisk;
            if (user.Infected) return InfectionStatus.Infected;
            return InfectionStatus.Healthy;
        }

        [Authorize(Roles = "Admin")]
        public JsonResult UsersComposition()
        {

            var healthy = 0;
            var infected = 0;
            var atRisk = 0;
            foreach (ApplicationUser User in _userInfoManager.Users())
            {
                if (User.Infected) infected++;
                else if (User.AtRisk) atRisk++;
                else healthy++;
            }

            return Json(new
            {
                Healthy = healthy,
                Infected = infected,
                AtRisk = atRisk,
            });

        }



        [Authorize(Roles = "Admin")]
        public JsonResult CheckinsTimeSeries()
        {


            var listOf = _context.Stay
                .Where(stay => Time.Now() < stay.TimeOfEntrance.AddYears(1))
                .GroupBy(stay => stay.TimeOfEntrance.Date)
                .Select(group => new
                {
                    x = group.Key,
                    y = group.Count()
                })
              ;
 

            return Json(listOf);

        }
    }
}
