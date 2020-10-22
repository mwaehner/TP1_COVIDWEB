using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TP1_ARQWEB.Areas.Identity.Data;


namespace TP1_ARQWEB.Models
{


    public class NegativeTest
    {

        public int Id { get; set; }


        [Required]
        public string ApplicationUserId { get; set; }


        [DisplayName("Fecha del Test")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime TestDate { get; set; }


    }
}
