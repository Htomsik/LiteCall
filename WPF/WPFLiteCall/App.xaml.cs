using System;
using System.Threading.Tasks;
using System.Windows;
using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.IOC;
using Core.Services.AppInfrastructure.FileServices;
using Core.Services.Interfaces.AppInfrastructure;
using Core.VMD.AppInfrastructure.Windows.MainWindow;
using LiteCall.IOC;
using LiteCall.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LiteCall;

public partial class App : Application
{

    #region Properies and Fields

    #region Host

    private static IHost? _host;

    private static  IHost Host => _host ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

    #endregion

    public static IServiceProvider Services => Host.Services;

    #region StartupWindow

    private static StartupWindow _startupWindow;

    private static StartupWindow StartupWindow => _startupWindow ??= new ();

    #endregion

    #endregion

   
    
    protected override async void OnStartup(StartupEventArgs e)
    {
        
     //   Services.GetRequiredService<SavedMainAccountFileSc>().GetDataFromFile();

     //   Services.GetRequiredService<SavedServersFileSc>().GetDataFromFile();

      //  await host.Services.GetRequiredService<ISyncDataOnServerSc>().GetFromServer();

        Services.GetRequiredService<INavigationServices>().Navigate();
          
        StartupWindow.Show();

        Task.WaitAll();
        
        await Task.Delay(600);

        MainWindow = Services.GetRequiredService<MainWindow>();

        MainWindow.Show();
        
        StartupWindow.Close();
        
        base.OnStartup(e);
        
        await Host.StartAsync().ConfigureAwait(false);
    }
    
    
    protected override async void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        await Host.StopAsync().ConfigureAwait(false);

        Host.Dispose();

        _host = null;
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
}