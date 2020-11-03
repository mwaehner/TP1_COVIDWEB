using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TP1_ARQWEB.Data;
using TP1_ARQWEB.Models;
using Microsoft.AspNetCore.Identity;
using TP1_ARQWEB.Areas.Identity.Data;
using Microsoft.Extensions.DependencyInjection;
using TP1_ARQWEB.PeriodicTasks;

internal interface IScopedProcessingService
{
    Task DoWork(CancellationToken stoppingToken);
}

internal class ScopedProcessingService : IScopedProcessingService
{
    
    private readonly StatusManager _statusManager;

    public ScopedProcessingService(UserManager<ApplicationUser> userManager, DBContext context)
    {
        _statusManager = new StatusManager(userManager, context);
    }


    public async Task DoWork(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _statusManager.UpdateUsersStatus();

            await Task.Delay(10000, stoppingToken);
        }
    }
}

public class ConsumeScopedServiceHostedService : BackgroundService
{

    public ConsumeScopedServiceHostedService(IServiceProvider services)
    {
        Services = services;
    }

    public IServiceProvider Services { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        

        using (var scope = Services.CreateScope())
        {
            var scopedProcessingService =
                scope.ServiceProvider
                    .GetRequiredService<IScopedProcessingService>();

            await scopedProcessingService.DoWork(stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
       

        await base.StopAsync(stoppingToken);
    }
}