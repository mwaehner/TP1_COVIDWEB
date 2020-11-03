using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP1_ARQWEB.Areas.Identity.Data;

namespace TP1_ARQWEB.Models
{
    public class AdministratorViewModel
    {

        public class UsuariosConLocaciones{
            public Location locaciones { get; set; }
            public ApplicationUser usuario { get; set; }
        }

        public List<UsuariosConLocaciones> Propietarios{ get; set; }
        
        public List<ApplicationUser> Everyone { get; set; }

        public List<Location> Locations { get; set; }

    }
}
