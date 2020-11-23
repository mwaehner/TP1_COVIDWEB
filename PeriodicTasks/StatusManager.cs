using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;
using Microsoft.AspNetCore.Identity;
using TP1_ARQWEB.Areas.Identity.Data;

namespace TP1_ARQWEB.PeriodicTasks
{
    public class StatusManager 
    {
        private readonly DBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StatusManager(UserManager<ApplicationUser> userManager, DBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        
        public async Task UpdateUsersStatus()
        {

            /*var location = _context.Location.Find(2);
            location.Capacidad++;
            _context.Update(location);
            await _context.SaveChangesAsync();


            foreach (ApplicationUser User in _userManager.Users)
            {
                
                if (User.InfectionStatus != InfectionStatus.Healthy && User.TimeOfLastCondition?.AddDays(15) < DateTime.Now)
                {
                    Notification.Type notificationType = 0;
                    if (User.InfectionStatus == InfectionStatus.AtRisk)
                        notificationType = Notification.Type.NoLongerAtRisk;
                    if (User.InfectionStatus == InfectionStatus.Infected)
                        notificationType = Notification.Type.NoLongerInfected;

                    User.InfectionStatus = InfectionStatus.Healthy;
                    User.TimeOfLastCondition = null;

                    await _userManager.UpdateAsync(User);

                    var newNotification = new Notification
                    {
                        UserId = User.Id,
                        NotificationType = notificationType,
                        Date = DateTime.Now
                    };
                    _context.Add(newNotification);
                    await _context.SaveChangesAsync();

                }
            }*/

        }

    }
}
