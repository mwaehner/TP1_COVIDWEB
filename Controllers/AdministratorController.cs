using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TP1_ARQWEB.Models;
using Microsoft.EntityFrameworkCore;
using TP1_ARQWEB.Areas.Identity.Data;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Services;

namespace TP1_ARQWEB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministratorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        private readonly DBContext _context;

        private UsuariosYLocs _repoUsers;

        public AdministratorController(
           UserManager<ApplicationUser> userManager, DBContext context)
        {
            _userManager = userManager;
            _context = context;
            _repoUsers = new UsuariosYLocs();
        }

        public IActionResult Index()
        { 
             
            return View();
        }
        public IActionResult DataUsers()
        {


            var model = new AdministratorViewModel
            {
                Propietarios = _repoUsers.usuariosPropietarios(_context),
               
            };
            return View(model);

            
        }
        
    }
}
