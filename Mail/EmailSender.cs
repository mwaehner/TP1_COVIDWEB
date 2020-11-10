using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.Net.Mail;

namespace TP1_ARQWEB.Mail
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.From = new MailAddress("covidwebARQ@gmail.com");
            mail.Subject = subject;
            mail.Body = htmlMessage;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("covidwebARQ@gmail.com", "covidweb1!"), // Enter seders User name and password  
                EnableSsl = true
            };

            smtp.Send(mail);

            return Task.CompletedTask;
        }
    }
}  