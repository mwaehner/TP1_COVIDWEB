using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP1_ARQWEB.Areas.Identity.Data;

namespace TP1_ARQWEB.Models
{
    public class AdministratorViewModel
    {



        public ApplicationUser[] Administrators { get; set; }
        public ApplicationUser[] Everyone { get; set; }

        public Location[] Locations { get; set; }

    }
}
