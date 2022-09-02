using System;
using System.Threading.Tasks;
using System.Windows;
using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Services.AppInfrastructure.FileServices;
using Core.Services.Interfaces.AppInfrastructure;
using Core.VMD.Windows;
using LiteCall.Services;
using LiteCall.Stores;
using LiteCall.ViewModels;
using LiteCall.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LiteCall;

public partial class App : Application
{
    private IHost? _host;

    public IHost Host => _host ?? Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

   
    protected override async void OnStartup(StartupEventArgs e)
    {
        var host = Host;

        host.Services.GetRequiredService<SavedMainAccountFileSc>().GetDataFromFile();

        host.Services.GetRequiredService<SavedServersFileSc>().GetDataFromFile();

        await host.Services.GetRequiredService<ISyncDataOnServerSc>().GetFromServer();

        var initialNavigationService = host.Services.GetRequiredService<INavigationServices>();

        initialNavigationService.Navigate();

        var startupWindow = new StartupWindow();

        startupWindow.Show();

        Task.WaitAll();

        await Task.Delay(2000);

        MainWindow = host.Services.GetRequiredService<MainWindow>();

        MainWindow.Show();

        startupWindow.Close();

        base.OnStartup(e);
        
        await host.StartAsync().ConfigureAwait(false);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        var host = Host;

        base.OnExit(e);

        await host.StopAsync().ConfigureAwait(false);

        host.Dispose();

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
            .RegisterStores()
            .RegisterVmd(host.Configuration);
    }
}