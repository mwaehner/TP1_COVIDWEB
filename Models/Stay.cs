using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP1_ARQWEB.Models
{
    public class Stay
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [Required]
        public int LocationId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Tiempo entrada")]
        public DateTime TimeOfEntrance { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Tiempo salida")]
        public DateTime? TimeOfExit { get; set; }


    }
}