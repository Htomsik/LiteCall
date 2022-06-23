using System;
using System.Threading.Tasks;
using System.Windows;
using LiteCall.Services;
using LiteCall.Services.FileServices;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels;
using LiteCall.Views;
using LiteCall.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LiteCall;

public partial class App : Application
{
    private IHost? _host;

    public  IHost Host => _host ?? Progam.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();


    protected override async void OnStartup(StartupEventArgs e)
    {
        var host = Host;

        host.Services.GetRequiredService<MainAccountFileServices>().GetDataFromFile();

        host.Services.GetRequiredService<ServersAccountsFileServices>().GetDataFromFile();

        await host.Services.GetRequiredService<ISynhronyzeDataOnServerServices>().GetFromServer();

        var initialNavigationService = host.Services.GetRequiredService<INavigationService>();

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