using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP1_ARQWEB.Models
{
    // por qué este nombre?
    public class MapLocation
    {

        public string Nombre { get; set; }
        public int Cap { get; set; }

        public int Conc { get; set; }

        public double Lat { get; set; }
        public double Lng { get; set; }
        

    }
}