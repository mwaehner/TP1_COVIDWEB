using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TP1_ARQWEB.Services;
using TP1_ARQWEB.Models;

namespace TP1_ARQWEB.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class

   

    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName ="nvarchar(100)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        //public int? CurrentLocationId { get; set; }
        public int? CurrentStayId { get; set; }

        public InfectionStatus InfectionStatus { get; set; }

        private bool FewerThan15DaysSinceCondition()
        {
            return TimeOfLastCondition?.AddDays(15) > Time.Now();
        }

        public bool Infected { get {

                return InfectionStatus == InfectionStatus.Infected && FewerThan15DaysSinceCondition();
            }
        }
        public bool AtRisk { get {
                return InfectionStatus == InfectionStatus.AtRisk && FewerThan15DaysSinceCondition();
            }
        }

        public bool Healthy
        {
            get
            {
                return !Infected && !AtRisk;
            }
        }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Tiempo salida")]
        public DateTime? TimeOfLastCondition { get; set; }
    }
}
