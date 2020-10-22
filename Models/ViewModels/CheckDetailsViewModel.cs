using System;

namespace TP1_ARQWEB.Models
{
    public class CheckDetailsViewModel
    {
        public Location location;
        public bool UserInLocation { get; set; }
        public bool UserAtRisk { get; set; }
        public bool UserInfected { get; set; }
        public bool LocationFull { get; set; }

    }
}
