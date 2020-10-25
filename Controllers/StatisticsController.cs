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

namespace TP1_ARQWEB.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly DBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StatisticsController(UserManager<ApplicationUser> userManager, DBContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        [Authorize]
        public IActionResult Index()
        {

            int locationsAmount = _context.Location.Count();
            int checkinsAmount = _context.Stay.Count();
            int usersAmount = _userManager.Users.Count();
            int infectionReportsAmount = _context.InfectionReport.Count();
            int negativeTestsAmount = _context.NegativeTest.Count();

            StatisticsViewModel statisticsModel = new StatisticsViewModel(usersAmount,
                locationsAmount,
                checkinsAmount,
                infectionReportsAmount,
                negativeTestsAmount);

            return View(statisticsModel);

        }
        [Authorize]
        public JsonResult UsersComposition()
        {

            var listOf = _userManager.Users
                .GroupBy(user => user.InfectionStatus)
                .Select(group => new
                {
                    Metric = group.Key,
                    Count = group.Count()
                });

            var healthy = listOf.FirstOrDefault(m => m.Metric == InfectionStatus.Healthy)?.Count ?? 0;
            var infected = listOf.FirstOrDefault(m => m.Metric == InfectionStatus.Infected)?.Count ?? 0;
            var atRisk = listOf.FirstOrDefault(m => m.Metric == InfectionStatus.AtRisk)?.Count ?? 0;


            return Json(new
            {
                Healthy = healthy,
                Infected = infected,
                AtRisk = atRisk,
            });

        }
        


        [Authorize]
        public JsonResult CheckinsTimeSeries()
        {


            var listOf = _context.Stay
                .Where(stay => DateTime.Now < stay.TimeOfEntrance.AddYears(1))
                .GroupBy(stay => stay.TimeOfEntrance.Date)
                .Select(group => new
                {
                    x = group.Key,
                    y = group.Count()
                })
              ;
 

            return Json(listOf );

        }
    }
}
