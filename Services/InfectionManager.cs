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

    public interface IInfectionManager
    {
        public Task UpdateRiskStatusFromStays(IQueryable<SimplifiedStay> stays, DateTime DiagnosisDate, string infectedUserId);
        public Task NewInfectionReport(ApplicationUser user, InfectionReport report);

    }
    public class InfectionManager : IInfectionManager
    {
        private readonly IUserInfoManager _userInfoManager;
        private readonly DBContext _context;
        private readonly INotificationManager _notificationManager;
        public InfectionManager(IUserInfoManager userInfoManager, DBContext context, INotificationManager notificationManager)
        {
            _userInfoManager = userInfoManager;
            _context = context;
            _notificationManager = notificationManager;
        }

        public async Task NewInfectionReport (ApplicationUser user, InfectionReport report)
        {
            if (user.Infected)
                throw new Exception("User already infected");

            report.ApplicationUserId = user.Id;
            report.DischargedDate = null;

            if (report.DiagnosisDate > Time.Now())
                throw new Exception("Diagnosis date more recent than Discharge date");

            _context.Add(report);
            await _context.SaveChangesAsync();

            await _userInfoManager.UpdateStatus(user, InfectionStatus.Infected, report.DiagnosisDate);

            var userStays = (from stay in _context.Stay
                            where stay.UserId == user.Id
                            select stay);


            await UpdateRiskStatusFromStays(userStays, report.DiagnosisDate, user.Id);

        }

        private bool intersectAtLeast15Minutes(SimplifiedStay stay1, SimplifiedStay stay2)
        {

            DateTime TimeOfExit1 = stay1.TimeOfExit ?? Time.Now();
            DateTime TimeOfExit2 = stay2.TimeOfExit ?? Time.Now();

            if (TimeOfExit1 < stay1.TimeOfEntrance.AddMinutes(15) || TimeOfExit2 < stay2.TimeOfEntrance.AddMinutes(15)) return false;
            if (stay1.TimeOfEntrance <= stay2.TimeOfEntrance && stay2.TimeOfEntrance.AddMinutes(15) <= TimeOfExit1) return true;
            if (stay2.TimeOfEntrance <= stay1.TimeOfEntrance && stay1.TimeOfEntrance.AddMinutes(15) <= TimeOfExit2) return true;
            return false;
        }

        private static DateTime timeEndOfRisk(SimplifiedStay stay1, SimplifiedStay stay2)
        {
            DateTime TimeOfExit1 = stay1.TimeOfExit ?? Time.Now();
            DateTime TimeOfExit2 = stay2.TimeOfExit ?? Time.Now();
            if (TimeOfExit1 < TimeOfExit2) return TimeOfExit1;
            return TimeOfExit2;
        }

        


        public async Task UpdateRiskStatusFromStays(IQueryable<SimplifiedStay> stays, DateTime DiagnosisDate, string infectedUserId = "")
        {

            var concurrentStays = from infectedStay in stays
                                  where (infectedStay.TimeOfExit == null || infectedStay.TimeOfExit > DiagnosisDate.AddDays(-15))
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
