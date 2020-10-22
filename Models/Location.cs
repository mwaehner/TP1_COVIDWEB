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
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Capacidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Latitud { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Longitud { get; set; }
        public int CantidadPersonasDentro { get; set; }
        public byte[] Image { get; set; }

    }
}