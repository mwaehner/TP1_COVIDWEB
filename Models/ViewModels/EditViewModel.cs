using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP1_ARQWEB.Models
{
    public class EditViewModel
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public int Capacidad { get; set; }

        [Required]
        public double Latitud { get; set; }
        [Required]
        public double Longitud { get; set; }

        [Required]
        public int AperturaHora { get; set; }
        [Required]
        public int AperturaMinuto { get; set; }
        [Required]
        public int CierreHora { get; set; }
        [Required]
        public int CierreMinuto { get; set; }

    }
}