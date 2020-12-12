using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;
using Microsoft.AspNetCore.Identity;
using TP1_ARQWEB.Areas.Identity.Data;
using Microsoft.AspNetCore.Http;
using QRCoder;
using TP1_ARQWEB.Services;

namespace TP1_ARQWEB.Services
{
    public interface ILocationService
    {
        public List<Location> GetLocationsForUser(ApplicationUser user);
        public Task<Location> GetLocationById(int? id, int? serverId = 2);
        public void AssertOwnership(Location location, ApplicationUser user);
        public string GetQrCode(Location location);
        public Task CreateNewLocation(Location location, ApplicationUser user);
        public Task EditLocation(Location location, Location editModel);
        public Task UpdateImage(Location location, IFormFile file);
        public Task RemoveLocation(Location location);
        public Task RemoveImage(Location location);
    }

    public class LocationService : ILocationService
    {
        private readonly DBContext _context;
        private readonly IExternalPlatformService _externalPlatformService;

        public LocationService(DBContext context, IExternalPlatformService externalPlatformService)
        {
            _context = context;
            _externalPlatformService = externalPlatformService;
        }

        public void AssertOwnership(Location location, ApplicationUser user)
        {
            if (location.IdPropietario != user.Id)
                throw new Exception("The user does not own this location");
        }

        
        public List<Location> GetLocationsForUser(ApplicationUser user)
        {
            return (from location in _context.Location
                    where location.IdPropietario == user.Id
                    select location).ToList<Location>();
        }

        

        public async Task<Location> GetLocationById(int? id, int? serverId = 2)
        {

            if (id == null || serverId == null) throw new Exception("Null id");
            if (_externalPlatformService.IsForeign((int)serverId))
                return await _externalPlatformService.GetLocation((int)id, (int)serverId);
            var location = await _context.FindAsync<Location>(id);
            if (location == null) throw new Exception("Location doesn't exist");
            return location;
        }
        public string GetQrCode(Location location)
        {
            var qrHandler = new QRHandler();
            return qrHandler.EncodeLocationIdToBase64(location.Id);
        }

        private void CheckLocationFields(Location location)
        {
            if (location.Latitud <= -90.0 || location.Latitud >= 90.0)
                throw new ModelException("Invalide latitude",
                    (modelState => { modelState.AddModelError("Latitud", "La latitud debe estar entre -90 y 90"); })
                    );
            if (location.Longitud >= 180.0 || location.Longitud <= -180.0)
                throw new ModelException("Invalide longitude",
                    (modelState => { modelState.AddModelError("Longitud", "La longitud debe estar entre -180 y 180"); })
                    );

            if (location.AperturaHora > 23 || location.AperturaHora < 0 || location.AperturaMinuto > 59 || location.AperturaMinuto < 0)
                throw new ModelException("Invalide opening hour",
                    (modelState => { modelState.AddModelError("AperturaHora", "Hora de apertura inválida"); })
                    );
            if (location.CierreHora > 23 || location.CierreHora < 0 || location.CierreMinuto > 59 || location.CierreMinuto < 0)
                throw new ModelException("Invalide closing hour",
                    (modelState => { modelState.AddModelError("CierreHora", "Hora de cierre inválida"); })
                    );
        }

        public async Task CreateNewLocation(Location location, ApplicationUser user)
        {
            
            try { CheckLocationFields(location); }
            catch { throw;  }
            
            location.IdPropietario = user.Id;
            _context.Add(location);
            await _context.SaveChangesAsync();

        }
        private bool LocationExists(int id)
        {
            return _context.Location.Any(e => e.Id == id);
        }
        public async Task EditLocation(Location location, Location editModel)
        {

            try { CheckLocationFields(editModel); }
            catch { throw; }

            try
            {

                location.Nombre = editModel.Nombre;
                location.Latitud = editModel.Latitud;
                location.Longitud = editModel.Longitud;
                location.Capacidad = editModel.Capacidad;
                location.AperturaHora = editModel.AperturaHora;
                location.AperturaMinuto = editModel.AperturaMinuto;
                location.CierreHora = editModel.CierreHora;
                location.CierreMinuto = editModel.CierreMinuto;

                _context.Update(location);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(location.Id))
                {
                    throw new ModelException("Location does not exist");
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ValidImageFormat(string imageName)
        {
            return imageName.EndsWith(".png") || imageName.EndsWith(".jpg");
        }

        private bool ValidImageSize(long imageSize)
        {
            return imageSize <= 1000000;
        }

        public async Task UpdateImage(Location location, IFormFile file)
        {
            if (file == null || file.Length <= 0)
                throw new ModelException("Invalid File");
            
            if (!ValidImageFormat(file.FileName))
                throw new ModelException("Invalid Format",
                    (ModelState => { ModelState.AddModelError("ImageName", "La imágen debe ser de formato PNG o JPG."); })
                    );
            if (!ValidImageSize(file.Length))
                throw new ModelException("Invalid Size",
                    (ModelState => { ModelState.AddModelError("ImageName", "La imágen es demasiado grande."); })
                    );


            location.Image = new byte[file.Length];

            file.OpenReadStream().Read(location.Image, 0, (int)file.Length);
            _context.Update(location);
            await _context.SaveChangesAsync();

            
        }

        public async Task RemoveImage(Location location)
        {
            location.Image = null;
            _context.Update(location);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveLocation(Location location)
        {
            _context.Location.Remove(location);
            await _context.SaveChangesAsync();
        }


    }

}
