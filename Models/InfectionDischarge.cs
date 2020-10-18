using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TP1_ARQWEB.Areas.Identity.Data;


namespace TP1_ARQWEB.Models
{
    public class InfectionDischarge
    {





        [Required]
        [DataType(DataType.Date)]
        public DateTime DischargedDate { get; set; }


        [Required]
        [DataType(DataType.Date)]
        public DateTime DiagnosisDate { get; set; }


        [Required]
        public int InfectionReportId { get; set; }

    }
}
