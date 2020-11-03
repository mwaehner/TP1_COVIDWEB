using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP1_ARQWEB.Areas.Identity.Data;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;
using static TP1_ARQWEB.Models.AdministratorViewModel;

namespace TP1_ARQWEB.Services
{
    public class UsuariosYLocs
    {
        public List<Location> locacionesActuales(DBContext context)
        {
            return context.Location.ToList();
        }

        public List<UsuariosConLocaciones> usuariosPropietarios(DBContext context)
        {
            var owners = context.Location.Join(context.Users, loc => loc.IdPropietario, usr => usr.Id, (loc, usr) => new { loc, usr }).ToArray();
            var allOwners = new List<UsuariosConLocaciones>();
            foreach(var ow in owners)
            {
                var userOwner = new UsuariosConLocaciones();
                userOwner.locaciones = ow.loc;
                userOwner.usuario = ow.usr;
                allOwners.Add(userOwner);
            }
            return allOwners;
        }
       
        public List<ApplicationUser> usuariosActuales(UserManager<ApplicationUser> userManager)
        {
            return userManager.Users.ToList();
        }

    }
}
