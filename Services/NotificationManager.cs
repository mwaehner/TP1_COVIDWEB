using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using TP1_ARQWEB.Areas.Identity.Data;
using System.Security.Claims;
using TP1_ARQWEB.Helpers;
using System.Net.Mail;
using TP1_ARQWEB.Mail;
using TP1_ARQWEB.Services;

namespace TP1_ARQWEB.Services
{

    public interface INotificationManager
    {
        public Task SendNotification(ApplicationUser user, Notification.Type type, DateTime time);
        public Task SendAtRiskEmail(ApplicationUser user, DateTime time);
    }
    public class NotificationManager : INotificationManager
    {

        private readonly DBContext _context;
        private readonly IEmailSender _emailSender;
        public NotificationManager(DBContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public async Task SendNotification(ApplicationUser user, Notification.Type type, DateTime time)
        {
            var newNotification = new Notification
            {
                NotificationType = type,
                Date = time,
                UserId = user.Id
            };

            _context.Notification.Add(newNotification);
            await _context.SaveChangesAsync();
        }

        public async Task SendAtRiskEmail(ApplicationUser user, DateTime time)
        {
            await _emailSender.SendEmailAsync(user.Email,
                                   "ADVERTENCIA: Riesgo de Contagio",
                                   "Se ha registrado que usted estuvo en contacto con alguien que recientemente contrajo CoronaVirus alrededor de la fecha" + time.ToString() + ". Por favor considere realizar un Test de CoronaVirus para asegurar su salud.");
        }

    }

}
