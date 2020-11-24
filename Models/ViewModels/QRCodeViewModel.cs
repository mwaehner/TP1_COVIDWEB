using System;

namespace TP1_ARQWEB.Models
{
    public class QRCodeViewModel
    {
        public Location location { get; set; }
        
        public string QREncodedBase64 { get; set; }
    }
}
