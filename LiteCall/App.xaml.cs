using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LiteCall.Services;
using LiteCall.Services.Authorisation;
using LiteCall.Services.Interfaces;
using LiteCall.Services.NavigationServices;
using LiteCall.Stores;
using LiteCall.Stores.ModelStores;
using LiteCall.Stores.NavigationStores;
using LiteCall.ViewModels;
using LiteCall.ViewModels.Base;
using LiteCall.ViewModels.Pages;
using LiteCall.ViewModels.ServerPages;
using LiteCall.Views;
using LiteCall.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LiteCall
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {


        private static IHost _Host;

        public  static IHost Host => _Host ?? Progam.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();
        
        
        protected override async void OnStartup(StartupEventArgs e)
        {

            var host = Host;

            host.Services.GetRequiredService<MainAccountFileServices>().GetDataFromFile();

            host.Services.GetRequiredService<ServersAccountsFileServices>().GetDataFromFile();




           var test =
               await host.Services.GetRequiredService<IhttpDataServices>().GetSaveServersUserOnMainServer(host.Services.GetRequiredService<AccountStore>().CurrentAccount, host.Services.GetRequiredService<ServersAccountsStore>().SavedServerAccounts
               );

           if (test != null)
           {
               host.Services.GetRequiredService<ServersAccountsStore>().SavedServerAccounts = test;
           }
            INavigationService InitialNavigationService = host.Services.GetRequiredService<INavigationService>();

            InitialNavigationService.Navigate();

            MainWindow = host.Services.GetRequiredService<MainWindov>();

            MainWindow.Show();

            base.OnStartup(e);

            
           await host.StartAsync().ConfigureAwait(false);


        }

        protected override async void OnExit(ExitEventArgs e)
        {
            var host = Host;

            //var accountStore = host.Services.GetRequiredService<AccountStore>();

            //var ServersStore = host.Services.GetRequiredService<ServersAccountsStore>();


            //host.Services.GetRequiredService<IhttpDataServices>().PostSaveServersUserOnMainServer(accountStore.CurrentAccount, ServersStore.SavedServerAccounts);

            base.OnExit(e);

            await host.StopAsync().ConfigureAwait(false);

            host.Dispose();

            _Host = null;
        }

        public static  void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            services.AddSingleton<MainWindov>(s => new MainWindov()
            {
                DataContext = s.GetRequiredService<MainWindowVMD>()
            });

            services
                .RegisterServices(host.Configuration)
                .RegisterStores()
                .RegisterVMD(host.Configuration);

        }
        
    }


}

