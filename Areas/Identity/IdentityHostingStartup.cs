﻿using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TP1_ARQWEB.Areas.Identity.Data;
using TP1_ARQWEB.Data;

[assembly: HostingStartup(typeof(TP1_ARQWEB.Areas.Identity.IdentityHostingStartup))]
namespace TP1_ARQWEB.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<MvcLocationContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("MvcLocationContext")));

                services.AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                }).AddEntityFrameworkStores<MvcLocationContext>();
            });
        }
    }
}