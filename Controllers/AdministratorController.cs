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
                /*Everyone = _repoUsers.noPropietarios(_context),*/
                
            };
            return View(model);

            
        }
        /*
                // GET: AdministratorController
                public ActionResult Index()
                {
                    return View();
                }

                // GET: AdministratorController/Details/5
                public ActionResult Details(int id)
                {
                    return View();
                }

                // GET: AdministratorController/Create
                public ActionResult Create()
                {
                    return View();
                }

                // POST: AdministratorController/Create
                [HttpPost]
                [ValidateAntiForgeryToken]
                public ActionResult Create(IFormCollection collection)
                {
                    try
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    catch
                    {
                        return View();
                    }
                }

                // GET: AdministratorController/Edit/5
                public ActionResult Edit(int id)
                {
                    return View();
                }

                // POST: AdministratorController/Edit/5
                [HttpPost]
                [ValidateAntiForgeryToken]
                public ActionResult Edit(int id, IFormCollection collection)
                {
                    try
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    catch
                    {
                        return View();
                    }
                }

                // GET: AdministratorController/Delete/5
                public ActionResult Delete(int id)
                {
                    return View();
                }

                // POST: AdministratorController/Delete/5
                [HttpPost]
                [ValidateAntiForgeryToken]
                public ActionResult Delete(int id, IFormCollection collection)
                {
                    try
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    catch
                    {
                        return View();
                    }
                }*/
    }
}
