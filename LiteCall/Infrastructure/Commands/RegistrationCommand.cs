using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using LiteCall.Infrastructure.Commands.Base;
using LiteCall.Model;
using LiteCall.Services;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;
using LiteCall.ViewModels.Pages;

namespace LiteCall.Infrastructure.Commands
{
    internal class RegistrationCommand<TViewModel> : AsyncCommandBase where TViewModel : RegistrationPageVMD
    {
        private readonly TViewModel _RegVMD;
    
        private readonly AccountStore _AccountStore;

        private readonly Func<object, bool> _CanExecute;
       

        public RegistrationCommand(string Captcha,TViewModel RegVMD,
            AccountStore accountStore, Action<Exception> onException, Func<object, bool> canExecute = null) : base(onException)
        {
           
            _RegVMD = RegVMD;
          
            _AccountStore = accountStore;
            _CanExecute = canExecute;
        }
        public override bool CanExecute(object parameter)
        {
            if (!IsExecuting)
            {
                return (_CanExecute?.Invoke(parameter) ?? true);
            }
            else
            {
                return false;
            }
        }
        protected override async Task ExecuteAsync(object parameter)
        {

            _RegVMD.ModalStatusMessage = "Connecting to server. . .";


            Account newAccount = new Account()
            {
                Login = _RegVMD.Login,
                Password = _RegVMD.Password,
            };

            var Response = await DataBaseService.Registration(newAccount, _RegVMD.CapthcaString);

            if (Response.Replace(" ","") == System.Net.HttpStatusCode.BadRequest.ToString())
            {
                _RegVMD.GetCaptcha();
                
                return;
            }
            else if (Response == System.Net.HttpStatusCode.Conflict.ToString())
            {
               _RegVMD.ModalStatus = false;

               _RegVMD.CapthcaString = string.Empty;
               return;
            }
            newAccount.Token = Response;
            newAccount.IsAuthorise = true;
            
            //Задержка перед открытием
            _RegVMD.ModalStatusMessage = "Registration sucsesfull. . .";
            await Task.Delay(1000);

            

            _RegVMD.ModalStatusMessage = string.Empty;

            _AccountStore.CurrentAccount = newAccount;





        }

    }
}
