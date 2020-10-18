using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TP1_ARQWEB.Areas.Identity.Data;

namespace TP1_ARQWEB.Models
{
    public class InfectionReport
    {

        public int Id { get; set; }


        [Required]
        public string ApplicationUserId { get; set;  }

    

        [DataType(DataType.Date)]
        [Required]
        public DateTime DiagnosisDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DischargedDate { get; set; }



    }
}
