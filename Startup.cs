using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using TP1_ARQWEB.Services;

namespace TP1_ARQWEB
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddTransient<IUserInfoManager, UserInfoManager>();
            services.AddTransient<INotificationManager, NotificationManager>();
            services.AddTransient<IInfectionService, InfectionService>();
            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<IExternalPlatformService, ExternalPlatformService>();
            services.AddScoped<ICheckService, CheckService>();


            services.AddDbContext<DBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DBContextConnection")));


            //services.AddHostedService<ConsumeScopedServiceHostedService>();
            //services.AddScoped<IScopedProcessingService, ScopedProcessingService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {


            //    if (!env.IsDevelopment())
            //  {
            app.UseDeveloperExceptionPage();
            //}
            //  else
            // {
            //   app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //   app.UseHsts();
            //}
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                // Desprolijo. Estas obligando a poner toda la api en un unico controller. 
                // Ser�a m�s l�gico admitir varios controllers dentro de api.
                endpoints.MapControllerRoute(
                    name: "api",
                    pattern: "api/action/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
