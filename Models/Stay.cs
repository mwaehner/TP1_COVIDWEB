using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP1_ARQWEB.Models
{
    
    public class ExternalStay
    {
        public int location_id { get; set; }
        public int server_id { get; set; }

        public string begin { get; set; }
        public string end { get; set; }

        public Stay ToLocalStay()
        {
            return new Stay()
            {
                UserId = "",
                LocationId = location_id,
                ServerId = server_id,
                TimeOfEntrance = Convert.ToDateTime(begin),
                TimeOfExit = Convert.ToDateTime(end)
            };
        }


    }

    public class ListOfExStays
    {
        public ExternalStay[] stays { get; set; }
    }

    public class Stay 
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [Required]
        public int LocationId { get; set; }
        [Required]
        public int ServerId { get; set; }

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