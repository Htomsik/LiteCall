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

namespace LiteCall
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly AccountStore _AccountStore;
        private readonly NavigationStore _NavigationStore;


        public App()
        {
            _NavigationStore = new NavigationStore();
            _AccountStore = new AccountStore();
        }
        protected override void OnStartup(StartupEventArgs e)
        {


            INavigatonService<AuthorisationPageVMD> AuthPageNavigationService = CreateAutPageNavigationServices();
            AuthPageNavigationService.Navigate();

            MainWindow = new MainWindov()
            {
                
                DataContext = new MainWindowVMD(_NavigationStore)
            };
            MainWindow.Show();

            base.OnStartup(e);
        }

        private INavigatonService<AuthorisationPageVMD> CreateAutPageNavigationServices()
        {
            return new NavigationServices<AuthorisationPageVMD>(_NavigationStore, () => new AuthorisationPageVMD(_AccountStore, CreateMainPageNavigationServices()));
        }

        private INavigatonService<MainPageVMD> CreateMainPageNavigationServices()
        {
            return new NavigationServices<MainPageVMD>(_NavigationStore, () => new MainPageVMD(_AccountStore));
        }
    }


}

