using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP1_ARQWEB.Models
{
    public class SimplifiedStay {
        public virtual string UserId { get; set; }
        public virtual int LocationId { get; set; }
        public virtual int ServerId { get; set; }
        public virtual DateTime TimeOfEntrance { get; set; }
        public virtual DateTime? TimeOfExit { get; set; }
    }
    public class Stay : SimplifiedStay
    {
        public int Id { get; set; }

        [Required]
        public override string UserId { get; set; }
        [Required]
        public override int LocationId { get; set; }
        [Required]
        public override int ServerId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Tiempo entrada")]
        public override DateTime TimeOfEntrance { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm:ss}", ApplyFormatInEditMode = true)]
        [Display(Name = "Tiempo salida")]
        public override DateTime? TimeOfExit { get; set; }


    }
}