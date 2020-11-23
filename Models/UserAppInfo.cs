using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TP1_ARQWEB.Services;

namespace TP1_ARQWEB.Models
{
    public enum InfectionStatus
    {
        Healthy = 0,
        Infected,
        AtRisk,
    }
    public class UserAppInfo
    {
        public string Id { get; set; }
        public int? CurrentLocationId { get; set; }
        public int? CurrentStayId { get; set; }

        public InfectionStatus InfectionStatus { get; set; }

        private bool FewerThan15DaysSinceCondition()
        {
            return TimeOfLastCondition?.AddDays(15) > Time.Now();
        }

        public bool Infected
        {
            get
            {

                return InfectionStatus == InfectionStatus.Infected && FewerThan15DaysSinceCondition();
            }
        }
        public bool AtRisk
        {
            get
            {
                return InfectionStatus == InfectionStatus.AtRisk && FewerThan15DaysSinceCondition();
            }
        }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Tiempo salida")]
        public DateTime? TimeOfLastCondition { get; set; }

        public string Email { get; set; }

    }
}