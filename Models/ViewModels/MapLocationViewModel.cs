using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP1_ARQWEB.Models
{
    // por qué este nombre?
    public class MapLocation
    {

        public string name { get; set; }
        public int concurrence { get; set; }
        public int capacity { get; set; }


        public double latitude { get; set; }
        public double longitude { get; set; }
        

    }
}