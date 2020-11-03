using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP1_ARQWEB.Models
{
    public class ImageViewModel
    {
        public string ImageName { get; set; }
        public long ImageSize { get; set; }

        public Location CurrentLocation { get; set; }

    }
}