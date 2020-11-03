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
            
        }

    }
}
