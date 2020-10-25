using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP1_ARQWEB.Models
{
    public class Notification
    {
        public enum Type
        {
            AtRisk
        }

        public Notification() { }

        public string GetNotificationString()
        {
            return GetNotificationStringFromType(this.NotificationType);
        }
        public static string GetNotificationStringFromType(Type type)
        {
            switch (type)
            {
                case Type.AtRisk:
                    return "Estas en riesgo, estuviste en contacto recientemente con alguien que dio positivo de coronavirus.";
                default:
                    return "";
            }
        }
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public Type NotificationType { get; set; }

        [DisplayName("Fecha de diagnóstico")]
        [DataType(DataType.Date)]
        [Required]
        public DateTime Date { get; set; }
    }
}