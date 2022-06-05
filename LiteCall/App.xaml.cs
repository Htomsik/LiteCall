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

            services.AddSingleton<NavigationStore>();

            services.AddSingleton<INavigationService>(s => CreateAutPageNavigationServices(s));


            services.AddTransient<AuthorisationPageVMD>(s => new AuthorisationPageVMD( s.GetRequiredService<AccountStore>(),
                CreateMainPageNavigationServices(s),
                CreateRegistrationPageNavigationServices(s)));


            services.AddTransient<RegistrationPageVMD>(s => new RegistrationPageVMD(
                s.GetRequiredService<AccountStore>(),
                CreateMainPageNavigationServices(s),CreateRegistrationPageNavigationServices(s)));

            services.AddTransient<MainPageVMD>(
                s => new MainPageVMD(s.GetRequiredService<AccountStore>()));

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

            return new NavigationServices<AuthorisationPageVMD>
            (serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<AuthorisationPageVMD>());
        }

        private INavigationService CreateRegistrationPageNavigationServices(IServiceProvider serviceProvider)
        {
            return new NavigationServices<RegistrationPageVMD>
                (serviceProvider.GetRequiredService<NavigationStore>(),
                    () => serviceProvider.GetRequiredService<RegistrationPageVMD>());
        }

        private INavigationService CreateMainPageNavigationServices(IServiceProvider serviceProvider)
        {
            return new NavigationServices<MainPageVMD>(serviceProvider.GetRequiredService<NavigationStore>(),
                () => serviceProvider.GetRequiredService<MainPageVMD>());
        }
    }


}

