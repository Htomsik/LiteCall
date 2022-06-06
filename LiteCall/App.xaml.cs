using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels;
using LiteCall.ViewModels.Base;
using LiteCall.ViewModels.Pages;
using LiteCall.Views;
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

            services.AddSingleton<MainWindowNavigationStore>();

            services.AddSingleton<AdditionalNavigationStore>();

            services.AddSingleton<SettingsAccNavigationStore>();



            services.AddSingleton<INavigationService>(s => CreateMainPageNavigationServices(s));

            
            services.AddTransient<AuthorisationPageVMD>(s => new AuthorisationPageVMD( s.GetRequiredService<AccountStore>(),
                CreateRegistrationPageNavigationServices(s)));


            services.AddSingleton<CloseAdditionalNavigationServices>();



            services.AddTransient<RegistrationPageVMD>(s => new RegistrationPageVMD(
                s.GetRequiredService<AccountStore>(),
                CreateMainPageNavigationServices(s),CreateAutPageNavigationServices(s)));




            services.AddTransient<MainPageVMD>(
                s => new MainPageVMD(s.GetRequiredService<AccountStore>(),
                    s.GetRequiredService<ServerAccountStore>(),
                    s.GetRequiredService<ServersAccountsStore>(),
                    CreateSettingPageNavigationService(s)));




            services.AddTransient<SettingVMD>(s => new SettingVMD(s.GetRequiredService<AccountStore>(), 
                s.GetRequiredService<CloseAdditionalNavigationServices>(), 
                CreateAutPageNavigationServices(s), s.GetRequiredService<SettingsAccNavigationStore>()
               ));



            services.AddSingleton<MainWindowVMD>();

            services.AddSingleton<MainWindov>(s => new MainWindov()
            {
                DataContext = s.GetRequiredService<MainWindowVMD>()
            });

           _ServicesPovider = services.BuildServiceProvider();

        }
        protected override void OnStartup(StartupEventArgs e)
        {


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

      
    }


}

