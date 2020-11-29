using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;

namespace TP1_ARQWEB.Services
{
    public interface ICheckService
    {
        public Task<CheckResult> Checkin(int id);
        public Task<CheckResult> Checkout(int id);
    }

    public class CheckService : ICheckService
    {
        private readonly DBContext _context;

        public CheckService(DBContext context)
        {
            _context = context;
        }

        public async Task<CheckResult> Checkin(int Id)
        {

            try
            {

                var location = await _context.Location.FindAsync(Id);

                if (location == null)
                {
                    return new CheckResult { successful = false, message = "La locacion no existe" };
                }


                if (location.CantidadPersonasDentro >= location.Capacidad)
                {

                    return new CheckResult { successful = false, message = String.Format("El sitio {0} esta lleno ", location.Nombre) };
                }

                location.CantidadPersonasDentro++;
                _context.Update(location);

                Stay newStay = new Stay
                {
                    UserId = null, // que hacer con el UserId.
                    LocationId = (int)Id,
                    TimeOfEntrance = Time.Now(),
                    TimeOfExit = null
                };

                _context.Add(newStay);

                await _context.SaveChangesAsync();

                return new CheckResult { successful = true, message = "" };

            }
            catch (DbUpdateConcurrencyException) { }

            return new CheckResult { successful = false, message = "Alguien más quiso entrar antes que vos" };
        }

        public async Task<CheckResult> Checkout(int id)
        {
            try
            {

                var location = await _context.Location.FindAsync(id);
                location.CantidadPersonasDentro--;

                _context.Update(location);



                // Turbio.
                // Lo mejor seria devolver el id de la stay en el checking y hacer checkout a partir de eso.
                // ...o guardarnos el id del "usuario externo"
                Stay currentStay = await _context.Stay
                .FirstOrDefaultAsync(m => m.LocationId == id && m.UserId == null && m.TimeOfExit == null);

                if (currentStay == null)
                {
                    return new CheckResult { successful = false, message = "Nunca ha ingresado en este local, no puede hacer checkout." };
                }

                currentStay.TimeOfExit = Time.Now();

                _context.Update(currentStay);

                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException) {
                return new CheckResult { successful = false, message = "Hubo otra operacion en simultaneo, vuelva a intentarlo." };

            }

            return new CheckResult { successful = true, message = "Checkout realizado con exito" };

        }
    }


    public class CheckResult
    {


        public bool successful { get; set; }
        public string message { get; set; }

    }

}
