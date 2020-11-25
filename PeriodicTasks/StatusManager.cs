using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;
using Microsoft.AspNetCore.Identity;
using TP1_ARQWEB.Areas.Identity.Data;
using TP1_ARQWEB.Services;

namespace TP1_ARQWEB.PeriodicTasks
{
    public class StatusManager 
    {
        private readonly DBContext _context;
        private readonly UserInfoManager _userInfoManager;

        public StatusManager(DBContext context)
        {
            _userInfoManager = new UserInfoManager(null, context);
            _context = context;
        }

        
        public async Task UpdateUsersStatus()
        {
            
            /*var location = _context.Location.Find(2);
            location.Capacidad++;
            _context.Update(location);
            await _context.SaveChangesAsync();*/
            
            

            /*foreach (UserAppInfo User in _userInfoManager.Users())
            {
                if (User.InfectionStatus != InfectionStatus.Healthy && User.TimeOfLastCondition?.AddDays(15) < DateTime.Now)
                {
                    User.InfectionStatus = InfectionStatus.Healthy;
                    User.TimeOfLastCondition = null;

                    await _userInfoManager.Update(User);

                }
            }*/
            
        }

    }
}
