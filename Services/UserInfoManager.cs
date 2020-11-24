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
    public class UserInfoManager
    {
        UserManager<ApplicationUser> _userManager;
        DBContext _context;
        public UserInfoManager(UserManager<ApplicationUser> userManager, DBContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        public async Task<UserAppInfo> FindUser(ClaimsPrincipal User)
        {
            var user = await _userManager.GetUserAsync(User);
            return await _context.UserAppInfo.FindAsync(user.Id);
        }

        public async Task<UserAppInfo> FindUserById(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            return await _context.UserAppInfo.FindAsync(user.Id);
        }

        public async Task<InfectionReport> GetOpenInfectionReport(string Id)
        {
            return await _context.InfectionReport
                    .FirstOrDefaultAsync(m => m.ApplicationUserId == Id && m.DischargedDate == null);
        }

        public async Task Update(UserAppInfo user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public DbSet<UserAppInfo> Users()
        {
            return _context.UserAppInfo;
        }

    }

}
