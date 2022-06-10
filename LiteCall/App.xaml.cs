using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LiteCall.Services;
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

namespace LiteCall
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        private readonly IServiceProvider _ServicesPovider;

        public App()
        {

            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<AccountStore>();

            services.AddSingleton<ServerAccountStore>();

            services.AddSingleton<ServersAccountsStore>();

            services.AddSingleton<CurrentServerStore>();


            services.AddSingleton<MainWindowNavigationStore>();

            services.AddSingleton<AdditionalNavigationStore>();

            services.AddSingleton<ModalNavigationStore>();

            services.AddSingleton<SettingsAccNavigationStore>();

            services.AddSingleton<MainPageServerNavigationStore>();

            services.AddSingleton<INavigationService>(s => CreateMainPageNavigationServices(s));

            
            services.AddTransient<AuthorisationPageVMD>(s => new AuthorisationPageVMD( s.GetRequiredService<AccountStore>(),
                CreateRegistrationPageNavigationServices(s)));


            services.AddSingleton<CloseAdditionalNavigationServices>();

            services.AddSingleton<CloseModalNavigationServices>();

            services.AddSingleton<RegistrationMainServerService>();

            services.AddSingleton<FileServices>(s => new FileServices(s.GetRequiredService<ServersAccountsStore>()));

            services.AddTransient<RegistrationPageVMD>(s => new RegistrationPageVMD(
                s.GetRequiredService<AccountStore>(),
                CreateAutPageNavigationServices(s), s.GetRequiredService<RegistrationMainServerService>()));

            

            services.AddTransient<SettingVMD>(s => new SettingVMD(s.GetRequiredService<AccountStore>(),s.GetRequiredService<ServersAccountsStore>(), 
                s.GetRequiredService<CloseAdditionalNavigationServices>(), 
                CreateAutPageNavigationServices(s), s.GetRequiredService<SettingsAccNavigationStore>()
               ));


      
            services.AddTransient<MainPageVMD>(
                s => new MainPageVMD(s.GetRequiredService<AccountStore>(),
                    s.GetRequiredService<ServerAccountStore>(),
                    s.GetRequiredService<ServersAccountsStore>(), s.GetRequiredService<CurrentServerStore>(), s.GetRequiredService<MainPageServerNavigationStore>(),
                    CreateSettingPageNavigationService(s), CreateServerPageNavigationService(s),CreateModalRegistrationPageNavigationServices(s)));


            services.AddTransient<ServerVMD>(s =>
                new ServerVMD(s.GetRequiredService<ServerAccountStore>(), s.GetRequiredService<CurrentServerStore>()));


            
            services.AddTransient<ServerRegistrationModalVMD>(s => new ServerRegistrationModalVMD(
                s.GetRequiredService<CloseModalNavigationServices>(),createApiRegistrationSevices(s),s.GetRequiredService<CurrentServerStore>()));




            services.AddSingleton<MainWindowVMD>();

            services.AddSingleton<MainWindov>(s => new MainWindov()
            {
                DataContext = s.GetRequiredService<MainWindowVMD>()
            });

           _ServicesPovider = services.BuildServiceProvider();

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            _ServicesPovider.GetRequiredService<FileServices>().GetAccountsServers();

            INavigationService InitialNavigationService = _ServicesPovider.GetRequiredService<INavigationService>();

            InitialNavigationService.Navigate();

            MainWindow = _ServicesPovider.GetRequiredService<MainWindov>();

            MainWindow.Show();

            base.OnStartup(e);


        }

        internal INavigationService CreateAutPageNavigationServices(IServiceProvider serviceProvider)
        {

            return new SettingAccNavigationServices<AuthorisationPageVMD>
            (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
                () => serviceProvider.GetRequiredService<AuthorisationPageVMD>());
        }

        private INavigationService CreateRegistrationPageNavigationServices(IServiceProvider serviceProvider)
        {
            return new SettingAccNavigationServices<RegistrationPageVMD>
                (serviceProvider.GetRequiredService<SettingsAccNavigationStore>(),
                    () => serviceProvider.GetRequiredService<RegistrationPageVMD>());
        }

        private INavigationService CreateMainPageNavigationServices(IServiceProvider serviceProvider)
        {
            return new NavigationServices<MainPageVMD>(serviceProvider.GetRequiredService<MainWindowNavigationStore>(),
                () => serviceProvider.GetRequiredService<MainPageVMD>());
        }

        private INavigationService CreateSettingPageNavigationService(IServiceProvider serviceProvider)
        {
            return new AdditionalNavigationServices<SettingVMD>(serviceProvider.GetRequiredService<AdditionalNavigationStore>(),() => serviceProvider.GetRequiredService<SettingVMD>());
        }

        internal INavigationService CreateServerPageNavigationService(IServiceProvider serviceProvider)
        {
            return new MainPageServerNavigationSevices<ServerVMD>(serviceProvider.GetRequiredService<MainPageServerNavigationStore>(), ()=>serviceProvider.GetRequiredService<ServerVMD>());
        }


        private INavigationService CreateModalRegistrationPageNavigationServices(IServiceProvider serviceProvider)
        {
            return new ModalNavigateServices<ServerRegistrationModalVMD>
            (serviceProvider.GetRequiredService<ModalNavigationStore>(),
                () => serviceProvider.GetRequiredService<ServerRegistrationModalVMD>());
        }


        private IRegistrationSevices createApiRegistrationSevices(IServiceProvider serviceProvider)
        {
            return new RegistrationApiServerServices(serviceProvider.GetRequiredService<ServersAccountsStore>(),
                serviceProvider.GetRequiredService<CurrentServerStore>(),serviceProvider.GetRequiredService<CloseModalNavigationServices>());
        }

    }


}

