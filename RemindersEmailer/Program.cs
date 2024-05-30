using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Logging;
using RemindersEmailer;
using RemindersEmailer.Data;
using RemindersEmailer.LoggerService;
using System;


public class Program
{
    static async Task Main(string[] args)
    {
        var logger = LogManager.GetCurrentClassLogger();
        LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "\nlog.config"));

        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        logger.Info("Configuring the setup...");

        var builder = new HostBuilder();

        builder.ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddDbContext<ReminderModuleDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("ReminderModuleContext")));
            services.AddHostedService<EmailJobScheduler>();
        });        

        logger.Info("Starting the service..");
        await builder.RunConsoleAsync();
        logger.Info("Shutting the service..");
        NLog.LogManager.Shutdown();
        
    }
}