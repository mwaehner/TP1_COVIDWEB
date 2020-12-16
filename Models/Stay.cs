using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TP1_ARQWEB.Models
{



    public class ExternalStay
    {
        public int location_id { get; set; }
        public int server_id { get; set; }

        public int begin { get; set; }
        public int end { get; set; }


        // from argentinan date time to unix timestamp
        public static int toUnixTime(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }

            DateTime v = (DateTime)dateTime;
            v = v.AddHours(3);

            return (int)v.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }


        // from unix timestamp to argnetian dateTime
        public static DateTime fromUnixTime(int unixTime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToUniversalTime();
            epoch = epoch.AddHours(-3);
            
            
            return epoch.AddSeconds(unixTime);
        }

        public Stay ToLocalStay()
        {
            return new Stay()
            {
                UserId = "",
                LocationId = location_id,
                ServerId = server_id,
                TimeOfEntrance = fromUnixTime(begin),
                TimeOfExit = fromUnixTime(end)
            };
        }

        internal static ExternalStay from(Stay aStay)
        {
            return new ExternalStay
            {
                location_id = aStay.LocationId,
                server_id = aStay.ServerId,
                begin = toUnixTime(aStay.TimeOfEntrance), // Change this
                end = toUnixTime(aStay.TimeOfExit),

            };
        }
    }

    public class ListOfExStays
    {
        public List<ExternalStay> stays { get; set; }


        public static ListOfExStays FromListOfStays(IQueryable<Stay> listOfStays)
        {



            List<ExternalStay> stays = listOfStays.Select(aStay => ExternalStay.from(aStay)).ToList();


            return new ListOfExStays { stays = stays };


        }
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