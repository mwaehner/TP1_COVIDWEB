using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TP1_ARQWEB.Areas.Identity.Data;


namespace TP1_ARQWEB.Models
{


    public class InfectionReport
    {

        public int Id { get; set; }


        [Required]
        public string ApplicationUserId { get; set; }


        [DisplayName("Fecha de diagnóstico")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime DiagnosisDate { get; set; }

        [DisplayName("Fecha de alta")]
        [DataType(DataType.Date)]
        public DateTime? DischargedDate { get; set; }



    }
}
