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

            services.AddSingleton<INavigationService>(s => CreateMainPageNavigationServices(s));

            #region Хранилища

            services.AddSingleton<AccountStore>();

            services.AddSingleton<SettingsStore>();

            services.AddSingleton<ServerAccountStore>();

            services.AddSingleton<ServersAccountsStore>();

            services.AddSingleton<CurrentServerStore>();

            #endregion

            #region NavigationStores
            services.AddSingleton<MainWindowNavigationStore>();

            services.AddSingleton<AdditionalNavigationStore>();

            services.AddSingleton<ModalNavigationStore>();

            services.AddSingleton<SettingsAccNavigationStore>();

            services.AddSingleton<MainPageServerNavigationStore>();
            #endregion

            #region Сервисы

            services.AddSingleton<ServersAccountsFileServices>(s => new ServersAccountsFileServices(s.GetRequiredService<ServersAccountsStore>()));

            services.AddSingleton<MainAccountFileServices>(s => new MainAccountFileServices(s.GetRequiredService<AccountStore>(),s.GetRequiredService<SettingsStore>()));

            services.AddSingleton<CloseAdditionalNavigationServices>();

            services.AddSingleton<CloseModalNavigationServices>();

            #endregion

            #region Регистрация/Авторизация на мейн сервере

            services.AddTransient<AuthorisationPageVMD>(s => new AuthorisationPageVMD(
                CreateRegistrationPageNavigationServices(s), CreateMainAuthorisationServices(s)));


            services.AddTransient<RegistrationPageVMD>(s => new RegistrationPageVMD(
                s.GetRequiredService<AccountStore>(),
                CreateAutPageNavigationServices(s), CreateMainRegistrationSevices(s)));

            #endregion

            #region Регистрация/Авторизация на API сервере чата

            services.AddTransient<ServerRegistrationModalVMD>(s => new ServerRegistrationModalVMD(
                CreateModalAuthorisationPageNavigationService(s),
                CreateApiRegistrationSevices(s),
                s.GetRequiredService<CurrentServerStore>()));


            services.AddTransient<ServerAuthorisationModalVMD>(s => new ServerAuthorisationModalVMD(
                CreateModalRegistrationPageNavigationServices(s), CreateApiAuthorisationServices(s)));


            #endregion


            services.AddTransient<MainPageVMD>(
                s => new MainPageVMD(s.GetRequiredService<AccountStore>(),
                    s.GetRequiredService<ServerAccountStore>(),
                    s.GetRequiredService<ServersAccountsStore>(), 
                    s.GetRequiredService<CurrentServerStore>(),
                    s.GetRequiredService<MainPageServerNavigationStore>(),
                    CreateSettingPageNavigationService(s), 
                    CreateServerPageNavigationService(s), 
                    CreateModalAuthorisationPageNavigationService(s),
                    CreateAuthCheckApiServerSevices(s)));

            services.AddTransient<SettingVMD>(s => new SettingVMD(s.GetRequiredService<AccountStore>(),s.GetRequiredService<ServersAccountsStore>(), 
                s.GetRequiredService<CloseAdditionalNavigationServices>(), 
                CreateAutPageNavigationServices(s), s.GetRequiredService<SettingsAccNavigationStore>()
               ));


            services.AddTransient<ServerVMD>(s =>
                new ServerVMD(s.GetRequiredService<ServerAccountStore>(), s.GetRequiredService<CurrentServerStore>()));



            services.AddSingleton<MainWindowVMD>(s => new MainWindowVMD(
                s.GetRequiredService<MainWindowNavigationStore>(),s.GetRequiredService<AdditionalNavigationStore>(),
                s.GetRequiredService<ModalNavigationStore>()
                ,s.GetRequiredService<CloseModalNavigationServices>()));

            services.AddSingleton<MainWindov>(s => new MainWindov()
            {
                DataContext = s.GetRequiredService<MainWindowVMD>()
            });

           _ServicesPovider = services.BuildServiceProvider();

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            _ServicesPovider.GetRequiredService<ServersAccountsFileServices>().GetDataFromFile();

            _ServicesPovider.GetRequiredService<MainAccountFileServices>().GetDataFromFile();

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

        #region Модальное окно

        private INavigationService CreateModalRegistrationPageNavigationServices(IServiceProvider serviceProvider)
        {
            return new ModalNavigateServices<ServerRegistrationModalVMD>
            (serviceProvider.GetRequiredService<ModalNavigationStore>(),
                () => serviceProvider.GetRequiredService<ServerRegistrationModalVMD>());
        }

        private INavigationService CreateModalAuthorisationPageNavigationService(IServiceProvider serviceProvider)
        {
            return new ModalNavigateServices<ServerAuthorisationModalVMD>(serviceProvider.GetRequiredService<ModalNavigationStore>(),() => serviceProvider.GetRequiredService<ServerAuthorisationModalVMD>());
        }


        #endregion


        #region Авторизация/Регистрация


        private IRegistrationSevices CreateApiRegistrationSevices(IServiceProvider serviceProvider)
        {
            return new RegistrationApiServerServices(serviceProvider.GetRequiredService<ServersAccountsStore>(),
                serviceProvider.GetRequiredService<CurrentServerStore>(), serviceProvider.GetRequiredService<CloseModalNavigationServices>());
        }

        private IRegistrationSevices CreateMainRegistrationSevices(IServiceProvider serviceProvider)
        {
            return new RegistrationMainServerService(serviceProvider.GetRequiredService<AccountStore>());
        }



        private IAuthorisationServices CreateApiAuthorisationServices(IServiceProvider serviceProvider)
        {
            return new AuthorisationApiServerServices(serviceProvider.GetRequiredService<ServersAccountsStore>(),
                serviceProvider.GetRequiredService<CurrentServerStore>(),
                serviceProvider.GetRequiredService<CloseModalNavigationServices>());
        }

        private IAuthorisationServices CreateMainAuthorisationServices(IServiceProvider serviceProvider)
        {
            return new AuthoisationMainServerServices(serviceProvider.GetRequiredService<AccountStore>());
        }

        private IAuthorisationServices CreateAuthCheckApiServerSevices(IServiceProvider serviceProvider)
        {
            return new AuthCheckApiServerSevices(serviceProvider.GetRequiredService<ServerAccountStore>());
        }

        #endregion

    }


}

