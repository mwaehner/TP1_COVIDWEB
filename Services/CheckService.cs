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
        public Task<CheckResult> Checkout(int? id, int serverId = 2);
        public Task<CheckResult> Checkout(ApplicationUser user);
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

        private async Task CreateStay(ApplicationUser user, int locationId, int serverId)
        {
            Stay newStay = new Stay
            {
                UserId = user.Id,
                LocationId = locationId,
                ServerId = serverId,
                TimeOfEntrance = Time.Now(),
                TimeOfExit = null
            };

            _context.Add(newStay);
            _context.SaveChanges();
            user.CurrentStayId = newStay.Id;
            await _userInfoManager.Update(user);

        }

        private async Task CloseStay (ApplicationUser user, Stay stay)
        {

            stay.TimeOfExit = Time.Now();

            _context.Update(stay);
            user.CurrentStayId = null;
            await _userInfoManager.Update(user);
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

                }

                if (_externalPlatformService.IsForeign(serverId))
                {
                    try
                    {
                        await _externalPlatformService.ExternalCheckIn(Id, serverId);
                    }
                    catch (Exception ex)
                    {
                        return new CheckResult { successful = false, message = ex.Message };
                    }
                }
                else
                {
                    location.CantidadPersonasDentro++;
                    _context.Update(location);

                    await _context.SaveChangesAsync();
                }

                if (user != null) await CreateStay(user, Id, serverId); ;

                return new CheckResult { successful = true, message = "Checkin realizado con exito" };

            }
            catch (DbUpdateConcurrencyException) { }

            return new CheckResult { successful = false, message = "Alguien más quiso entrar antes que vos" };
        }


        public async Task<CheckResult> Checkout(int? id, int serverId = 2)
        {
            if (id == null) return new CheckResult { successful = false, message = "Null location ID" };
            try
            {
                Location location;
                location = await _locationService.GetLocationById(id, serverId);

                if (_externalPlatformService.IsForeign(serverId))
                {
                    await _externalPlatformService.ExternalCheckOut((int)id, serverId);
                }
                else
                {
                    if (location.CantidadPersonasDentro > 0) location.CantidadPersonasDentro--;

                    _context.Update(location);

                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                return new CheckResult { successful = false, message = "Hubo otra operacion en simultaneo, vuelva a intentarlo." };

            }
            catch (Exception ex) { new CheckResult { successful = false, message = ex.Message }; }

            return new CheckResult { successful = true, message = "Checkout realizado con exito" };
        }
        public async Task<CheckResult> Checkout(ApplicationUser user)
        {
            try
            {

                var currentStay = _userInfoManager.GetOpenStay(user);

                if (currentStay == null) throw new Exception("User isn't currently in a location");

                Location location;
                location = await _locationService.GetLocationById(currentStay.LocationId, currentStay.ServerId);

                if (_externalPlatformService.IsForeign(currentStay.ServerId))
                {
                    await _externalPlatformService.ExternalCheckOut(currentStay.LocationId, currentStay.ServerId);
                }
                else
                {
                    if (location.CantidadPersonasDentro > 0) location.CantidadPersonasDentro--;

                    _context.Update(location);

                    await _context.SaveChangesAsync();
                }

                await CloseStay(user, currentStay);
                


            }
            catch (DbUpdateConcurrencyException) {
                return new CheckResult { successful = false, message = "Hubo otra operacion en simultaneo, vuelva a intentarlo." };

            } catch (Exception ex) { new CheckResult { successful = false , message =ex.Message}; }

            return new CheckResult { successful = true, message = "Checkout realizado con exito" };

        }
    }


    public class CheckResult
    {

        public bool successful { get; set; }
        public string message { get; set; }

    }

}
