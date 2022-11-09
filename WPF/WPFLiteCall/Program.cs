using System;
using Core.IOC;
using Core.VMD.AppInfrastructure.Windows.MainWindow;
using LiteCall.IOC;
using LiteCall.Views.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace LiteCall;

public class Program
{
    [STAThread]
    public static void Main()
    {
        var app = new App();

        app.InitializeComponent();

        app.Run();
    }

    #region Methods

    public static IHostBuilder CreateHostBuilder(string[] Args)
    {
        var hostBuilder = Host.CreateDefaultBuilder(Args)
            .UseContentRoot(Environment.CurrentDirectory)
            .ConfigureServices(ConfigureServices)
            .UseSerilog((context, services, configuration) =>
            {
                configuration
                    .WriteTo.File(@"logs\Log-.txt", rollingInterval: RollingInterval.Day,restrictedToMinimumLevel: LogEventLevel.Error)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
            })
            .ConfigureAppConfiguration((host, cfg) => cfg
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("apsettings.json", true, true)
                .AddUserSecrets<Program>(true)
            );


        return hostBuilder;
    }
    
    public static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
    {
        services.AddSingleton(s => new MainWindow
        {
            DataContext = s.GetRequiredService<MainWindowVmd>()
        });

        services
            .RegisterServices(host.Configuration)
            .RegisterWpfServices()
            .RegisterStores()
            .RegisterVmd(host.Configuration);
    }

    #endregion
    
}