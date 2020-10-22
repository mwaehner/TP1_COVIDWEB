using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;

namespace TP1_ARQWEB.Controllers
{
    public class ImageController : Controller
    {

        private readonly DBContext _context;
        public ImageController(DBContext context)
        {
            _context = context;
        }
        public async Task<ActionResult> ShowLocationImage(int id)
        {
            var location = await _context.Location.FindAsync(id);

            var imageData = location.Image;

            return File(imageData, "image/jpg");
        }
    }
}
