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
    public interface ICheckService
    {
        public Task<CheckResult> Checkin(int id, ApplicationUser user = null, int serverId = 2);
        public Task<CheckResult> Checkout(int? id, ApplicationUser user = null, int serverId = 2);
    }

    public class CheckService : ICheckService
    {
        private readonly DBContext _context;
        private readonly ILocationService _locationService;
        private readonly IUserInfoManager _userInfoManager;
        private readonly IExternalPlatformService _externalPlatformService;

        public CheckService(DBContext context, ILocationService locationService, IUserInfoManager userInfoManager, IExternalPlatformService externalPlatformService)
        {
            _context = context;
            _locationService = locationService;
            _userInfoManager = userInfoManager;
            _externalPlatformService = externalPlatformService;
        }

        private int CreateStay(ApplicationUser user, int locationId)
        {
            Stay newStay = new Stay
            {
                UserId = user.Id, 
                LocationId = locationId,
                TimeOfEntrance = Time.Now(),
                TimeOfExit = null
            };

            _context.Add(newStay);
            _context.SaveChanges();
            return newStay.Id;

        }

        private void CloseStay (ApplicationUser user, int locationId)
        {
            if (user.CurrentStayId == null)
                throw new Exception("User isn't in a location");
            Stay currentStay = _context.Stay
                .Find(user.CurrentStayId);

            if (currentStay == null)
                throw new Exception("User isn't in a location");

            if (currentStay.LocationId != locationId)
                throw new Exception("User isn't currently in this location");

            currentStay.TimeOfExit = Time.Now();

            _context.Update(currentStay);
        }


        public async Task<CheckResult> Checkin(int Id, ApplicationUser user = null, int serverId = 2)
        {

            try
            {
                Location location;
                try { location = await _locationService.GetLocationById(Id,serverId); }
                catch (Exception ex)
                {
                    return new CheckResult { successful = false, message = ex.Message };
                }

                if (location.CantidadPersonasDentro >= location.Capacidad)
                {
                    return new CheckResult { successful = false, message = String.Format("El sitio {0} esta lleno ", location.Nombre) };
                }

                if (user != null)
                {
                    var Result = new CheckResult { successful = true, message = "" };

                    if (user.Infected)
                    {
                        Result.successful = false;
                        Result.message = "User is infected and can't check in";
                    } else if (user.CurrentStayId != null)
                    {
                        Result.successful = false;
                        Result.message = "User is already checked in at a location";
                    }

                    if (!Result.successful) return Result;

                    int newStayId = CreateStay(user, Id);
                    user.CurrentLocationId = Id;
                    user.CurrentStayId = newStayId;
                    await _userInfoManager.Update(user);
                }

                if (_externalPlatformService.IsForeign(serverId))
                {
                    try {
                        return await _externalPlatformService.ExternalCheckIn(Id, serverId);
                    }
                    catch (Exception ex)
                    {
                        return new CheckResult { successful = false, message = ex.Message };
                    }
                } else
                {
                    location.CantidadPersonasDentro++;
                    _context.Update(location);

                    await _context.SaveChangesAsync();
                }

                

                return new CheckResult { successful = true, message = "Checkin realizado con exito" };

            }
            catch (DbUpdateConcurrencyException) { }

            return new CheckResult { successful = false, message = "Alguien más quiso entrar antes que vos" };
        }

        public async Task<CheckResult> Checkout(int? id, ApplicationUser user = null, int serverId = 2)
        {
            if (id == null) return new CheckResult {successful = false,  message = "Null location ID" };
            try
            {
                Location location;
                try { 

                    location = await _locationService.GetLocationById(id, serverId);
                    if (user != null)
                    {
                        CloseStay(user, (int)id);
                        user.CurrentLocationId = null;
                        user.CurrentStayId = null;
                        await _userInfoManager.Update(user);
                    }

                } catch(Exception ex)
                {
                    return new CheckResult { successful = false, message = ex.Message };
                }

                if (_externalPlatformService.IsForeign(serverId))
                {
                    return await _externalPlatformService.ExternalCheckOut((int)id, serverId);
                } else
                {
                    if (location.CantidadPersonasDentro > 0) location.CantidadPersonasDentro--;

                    _context.Update(location);

                    await _context.SaveChangesAsync();
                }


            }
            catch (DbUpdateConcurrencyException) {
                return new CheckResult { successful = false, message = "Hubo otra operacion en simultaneo, vuelva a intentarlo." };

            } catch (Exception) { new CheckResult { successful = false , message =""}; }

            return new CheckResult { successful = true, message = "Checkout realizado con exito" };

        }
    }


    public class CheckResult
    {

        public bool successful { get; set; }
        public string message { get; set; }

    }

}
