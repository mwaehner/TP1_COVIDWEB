using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TP1_ARQWEB.Services;

namespace TP1_ARQWEB.Models
{
    public class GeneralizedLocation
    {

        public string name { get; set; }
        public int concurrence { get; set; }
        public int capacity { get; set; }

        public double latitude { get; set; }
        public double longitude { get; set; }

        public Location ToLocation()
        {
            return new Location
            {
                Nombre = name,
                CantidadPersonasDentro = concurrence,
                Capacidad = capacity,
                Latitud = latitude,
                Longitud = longitude
            };
        }

    }
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

        [Required]
        public int AperturaHora { get; set; }
        [Required]
        public int AperturaMinuto { get; set; }
        [Required]
        public int CierreHora { get; set; }
        [Required]
        public int CierreMinuto { get; set; }

        public bool AbreATodaHora()
        {
            return AperturaHora == CierreHora && AperturaMinuto == CierreMinuto;
        }

        public bool AbreAMedianoche()
        {
            return AperturaHora > CierreHora
                || (AperturaHora == CierreHora && AperturaMinuto >= CierreMinuto);
        }

        public bool Abierto()
        {

            var horaActual = Time.Now().Hour;
            var minutoActual = Time.Now().Minute;

            if (AbreATodaHora()) return true;

            bool despuesDeApertura = horaActual > AperturaHora
                || horaActual == AperturaHora && minutoActual >= AperturaMinuto;
            bool antesDeCierre = horaActual < CierreHora
                || horaActual == CierreHora && minutoActual < CierreMinuto;

            if (AbreAMedianoche()) return despuesDeApertura || antesDeCierre;
            else return despuesDeApertura && antesDeCierre;

        }

        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}