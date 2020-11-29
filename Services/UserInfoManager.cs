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

    public interface IUserInfoManager
    {
        public Task<ApplicationUser> FindUser(ClaimsPrincipal User);
        public Task<ApplicationUser> FindUserById(string Id);
        public Task<InfectionReport> GetOpenInfectionReport(ApplicationUser user);
        public Task Update(ApplicationUser user);
        public IQueryable<ApplicationUser> Users();
        public Task UpdateStatus(ApplicationUser user, InfectionStatus stat, DateTime? timeOfStat);
    }
    public class UserInfoManager : IUserInfoManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DBContext _context;
        public UserInfoManager(UserManager<ApplicationUser> userManager, DBContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        public async Task<ApplicationUser> FindUser(ClaimsPrincipal User)
        {
            return await _userManager.GetUserAsync(User);
        }

        public async Task<ApplicationUser> FindUserById(string Id)
        {
            return await _userManager.FindByIdAsync(Id);
        }

        public async Task<InfectionReport> GetOpenInfectionReport(ApplicationUser user)
        {
            return await _context.InfectionReport
                    .FirstOrDefaultAsync(m => m.ApplicationUserId == user.Id && m.DischargedDate == null);
        }

        public async Task Update(ApplicationUser user)
        {
            await _userManager.UpdateAsync(user);
        }

        public IQueryable<ApplicationUser> Users()
        {
            return _userManager.Users;
        }

        public async Task UpdateStatus(ApplicationUser user, InfectionStatus stat, DateTime? timeOfStat)
        {
            user.InfectionStatus = stat;
            user.TimeOfLastCondition = timeOfStat;
            await _userManager.UpdateAsync(user);
        }

    }

}
