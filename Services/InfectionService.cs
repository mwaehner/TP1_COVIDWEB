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

namespace TP1_ARQWEB.Services
{

    public interface IInfectionService
    {
        public Task UpdateRiskStatusFromStays(IQueryable<Stay> stays, string infectedUserId = "");
        public Task NewInfectionReport(ApplicationUser user, InfectionReport report);
        public Task DischargeUser(ApplicationUser user, InfectionDischarge report);
        public Task NewNegativeTest(ApplicationUser user, NegativeTest test);

    }
    public class InfectionService : IInfectionService
    {
        private readonly IUserInfoManager _userInfoManager;
        private readonly DBContext _context;
        private readonly INotificationManager _notificationManager;
        private readonly IExternalPlatformService _externalPlatformService;
        public InfectionService(IUserInfoManager userInfoManager, DBContext context, INotificationManager notificationManager, IExternalPlatformService externalPlatformService)
        {
            _userInfoManager = userInfoManager;
            _context = context;
            _notificationManager = notificationManager;
            _externalPlatformService = externalPlatformService;
        }


        public async Task NewNegativeTest(ApplicationUser user, NegativeTest test)
        {
            if (!user.AtRisk)
                throw new Exception("User not currently at risk");

            test.ApplicationUserId = user.Id;

            if (test.TestDate > Time.Now())
                throw new ModelException("Test can't be more recent than present date",
                    (ModelState => { ModelState.AddModelError("TestDate", "La fecha de realización del test no puede ser posterior a la fecha actual."); })
                    );
            if (test.TestDate < user.TimeOfLastCondition)
                throw new ModelException("Test should be more recent than last time put into risk",
                    (ModelState => { ModelState.AddModelError("TestDate", "La fecha de realización del test debe ser posterior a la última vez que fue puesto en riesgo."); })
                    );

            _context.Add(test);
            await _context.SaveChangesAsync();

            await _userInfoManager.UpdateStatus(user,InfectionStatus.Healthy,null);
        }

        public async Task DischargeUser(ApplicationUser user, InfectionDischarge report)
        {
            var infectionReport = await _context.InfectionReport.FindAsync(report.InfectionReportId);

            if (infectionReport == null)
                throw new ModelException("Previous infection report not found");

            if (infectionReport.DiagnosisDate >= report.DischargedDate)
                throw new ModelException("Discharge date should be subsequent to the Diagnosis date",
                    (ModelState => { ModelState.AddModelError("DischargedDate", "La fecha de alta debe ser posterior a la de diagnostico"); })
                    );
            if (report.DischargedDate > Time.Now())
                throw new ModelException("Discharge date can't be more recent than present date",
                    (ModelState => { ModelState.AddModelError("DischargedDate", "La fecha de dada de alta no puede ser posterior a la fecha actual."); })
                    );

            infectionReport.DischargedDate = report.DischargedDate;

            _context.Update(infectionReport);
            await _context.SaveChangesAsync();

            await _userInfoManager.UpdateStatus(user,InfectionStatus.Healthy,null);
        }


        public async Task NewInfectionReport (ApplicationUser user, InfectionReport report)
        {
            if (user.Infected)
                throw new ModelException("User already infected");

            report.ApplicationUserId = user.Id;
            report.DischargedDate = null;

            if (report.DiagnosisDate > Time.Now())
                throw new ModelException("Diagnosis date more recent than Discharge date",
                    (ModelState => { ModelState.AddModelError("DiagnosisDate", "La fecha de diagnosis no puede ser posterior a la fecha actual."); })
                    );

            _context.Add(report);
            await _context.SaveChangesAsync();

            await _userInfoManager.UpdateStatus(user, InfectionStatus.Infected, report.DiagnosisDate);

            var userStays = (from stay in _context.Stay
                            where stay.UserId == user.Id &&
                            (stay.TimeOfExit == null || stay.TimeOfExit > report.DiagnosisDate.AddDays(-15))
                            select stay);


            await UpdateRiskStatusFromStays(userStays, user.Id);


            await notifyOtherServers(userStays);

        }

        private Task notifyOtherServers(IQueryable<Stay> userStays)
        {
            return _externalPlatformService.notifyOtherServers(userStays);
        }

        private bool intersectAtLeast15Minutes(Stay stay1, Stay stay2)
        {

            DateTime TimeOfExit1 = stay1.TimeOfExit ?? Time.Now();
            DateTime TimeOfExit2 = stay2.TimeOfExit ?? Time.Now();

            if (TimeOfExit1 < stay1.TimeOfEntrance.AddMinutes(15) || TimeOfExit2 < stay2.TimeOfEntrance.AddMinutes(15)) return false;
            if (stay1.TimeOfEntrance <= stay2.TimeOfEntrance && stay2.TimeOfEntrance.AddMinutes(15) <= TimeOfExit1) return true;
            if (stay2.TimeOfEntrance <= stay1.TimeOfEntrance && stay1.TimeOfEntrance.AddMinutes(15) <= TimeOfExit2) return true;
            return false;
        }

        private static DateTime timeEndOfRisk(Stay stay1, Stay stay2)
        {
            DateTime TimeOfExit1 = stay1.TimeOfExit ?? Time.Now();
            DateTime TimeOfExit2 = stay2.TimeOfExit ?? Time.Now();
            if (TimeOfExit1 < TimeOfExit2) return TimeOfExit1;
            return TimeOfExit2;
        }


        public async Task UpdateRiskStatusFromStays(IQueryable<Stay> stays, string infectedUserId = "")
        {

            var concurrentStays = from infectedStay in stays
                                join otherStay in _context.Stay 
                                on new { LI = infectedStay.LocationId, SI = infectedStay.ServerId }
                                equals new { LI = otherStay.LocationId, SI = otherStay.ServerId }
                                where otherStay.UserId != infectedUserId
                                  select new { 
                                    infectedStay = infectedStay,
                                    otherStay = otherStay };

            var usersToNotify = concurrentStays.AsEnumerable()
                                    .Where(x => intersectAtLeast15Minutes(x.infectedStay, x.otherStay))
                                    .OrderByDescending(x => timeEndOfRisk(x.infectedStay, x.otherStay))
                                    .Select(x => new { 
                                        userId = x.otherStay.UserId,
                                        timeEndOfRisk = timeEndOfRisk(x.infectedStay,x.otherStay)
                                    });

            var userList = usersToNotify.Select(s => s).ToList();



            foreach (var toNotify in userList)
            {
                var User = await _userInfoManager.FindUserById(toNotify.userId);

                if (User.Healthy || User.AtRisk && User.TimeOfLastCondition < toNotify.timeEndOfRisk)
                {
                    await _userInfoManager.UpdateStatus(User,InfectionStatus.AtRisk,toNotify.timeEndOfRisk);
                    await _notificationManager.SendNotification(User, Notification.Type.AtRisk, toNotify.timeEndOfRisk);
                    await _notificationManager.SendAtRiskEmail(User, toNotify.timeEndOfRisk);
                }

            }
        }

    }

}
