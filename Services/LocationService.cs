using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;
using TP1_ARQWEB.Areas.Identity.Data;

namespace TP1_ARQWEB.Services
{
    public interface ILocationService
    {
        public List<Location> GetLocationsForUser(ApplicationUser user);
        public Task<Location> GetLocationById(int? id);
        public void AssertOwnership(Location location, ApplicationUser user);
        public string GetQrCode(Location location);
        public Task CreateNewLocation(Location location, ApplicationUser user);
    }

    public class LocationService : ILocationService
    {
        private readonly DBContext _context;
        private readonly IUserInfoManager _userInfoManager;

        public LocationService(DBContext context, IUserInfoManager userInfoManager)
        {
            _context = context;
            _userInfoManager = userInfoManager; 
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
        public async Task<Location> GetLocationById(int? id)
        {
            if (id == null) throw new Exception("Null id");
            return await _context.FindAsync<Location>(id);
        }
        public string GetQrCode(Location location)
        {
            var qrHandler = new QRHandler();
            return qrHandler.EncodeLocationIdToBase64(location.Id);
        }

        public async Task CreateNewLocation(Location location, ApplicationUser user)
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

            
            location.IdPropietario = user.Id;
            _context.Add(location);
            await _context.SaveChangesAsync();

        }

    }

}
