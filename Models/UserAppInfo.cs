using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP1_ARQWEB.Models
{
    public class UserAppInfo
    {
        public string Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }

        [Column(TypeName = "int")]
        public int? CurrentLocation { get; set; }

    }
}