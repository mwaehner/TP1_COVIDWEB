﻿using System;
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
            var horaActual = DateTime.Now.Hour;
            var minutoActual = DateTime.Now.Minute;

            if (AbreATodaHora()) return true;

            if (AbreAMedianoche())
                return horaActual < CierreHora || horaActual > AperturaHora
                    || horaActual == CierreHora && minutoActual < CierreMinuto
                    || horaActual == AperturaHora && minutoActual >= AperturaMinuto;


            return horaActual < CierreHora && horaActual > AperturaHora
                || horaActual == AperturaHora && minutoActual > AperturaMinuto
                || horaActual == CierreHora && minutoActual < CierreMinuto;
        }

        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}