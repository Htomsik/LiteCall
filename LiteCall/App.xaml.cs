using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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

        protected override void OnStartup(StartupEventArgs e)
        {

            NavigationStore navigationStore = new NavigationStore();

            navigationStore.MainWindowCurrentViewModel = new MainPageVMD();



            MainWindow = new MainWindov()
            {
                DataContext = new MainWindowVMD(navigationStore)
            };

            MainWindow.Show();

            base.OnStartup(e);

        }
    }


}

