using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Model;
using LiteCall.ViewModels.Base;

namespace LiteCall.ViewModels.Pages
{
    internal class MainPageVMD:BaseVMD
    {
        public MainPageVMD()
        {
            VisibilitySwitchCommand = new LambdaCommand(OnVisibilitySwitchExecuted);
        }

        #region Данные с окна

        public ICommand VisibilitySwitchCommand { get; }
        private void OnVisibilitySwitchExecuted(object p)
        {
            if (Convert.ToInt32(p) == 1)
                VisibilitiStatus = Visibility.Collapsed;
            else
                VisibilitiStatus = Visibility.Visible;
        }


        public ObservableCollection<Server> ServersMark { get; }
        private Server _CurrentServer;
        public Server CurrentServer
        {
            get => _CurrentServer;
            set => Set(ref _CurrentServer, value);
        }


        private Visibility _VisibilitiStatus = Visibility.Collapsed;
        public Visibility VisibilitiStatus
        {
            get => _VisibilitiStatus;
            set => Set(ref _VisibilitiStatus, value);
        }


        private UserControl _selectedViewModel;
        public UserControl selectedViewModel
        {
            get => _selectedViewModel;
            set => Set(ref _selectedViewModel, value);
        }

        #endregion

    }
}
