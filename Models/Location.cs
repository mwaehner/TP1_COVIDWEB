using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP1_ARQWEB.Models
{
    public class Location
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string IdPropietario { get; set; }

        [Required]
        public int Capacidad { get; set; }

        [Required]
        public double Latitud { get; set; }
        [Required]
        public double Longitud { get; set; }
        public int CantidadPersonasDentro { get; set; }
        public byte[] Image { get; set; }

    }
}