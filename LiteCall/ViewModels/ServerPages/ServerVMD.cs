using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using AudioLib;
using LiteCall.Model;
using LiteCall.Stores;
using LiteCall.ViewModels.Base;

namespace LiteCall.ViewModels.ServerPages
{
    internal class ServerVMD:BaseVMD
    {
        public ServerVMD(AccountStore AccountStore,string ServerAdress)
        {
            _Account = AccountStore.CurrentAccount;


            netTcpBinding.Security.Mode = SecurityMode.None;
            netTcpBinding.Security.Transport.SslProtocols = System.Security.Authentication.SslProtocols.None;
            factory = new DuplexChannelFactory<IChatService>(context, netTcpBinding, "net.tcp://"+ServerAdress+"/WPFChat");
            server = factory.CreateChannel();

            server.Join(Account.Login);
            
        }

        public override void Dispose()
        {
            server.Disconect();
            base.Dispose();
        }

        #region Данные с формы

        public ObservableCollection<ServerUser> Users { get; }

        #endregion


        #region Хранилища

        private Account _Account;

        public Account Account
        {
            get => _Account;
            set => Set(ref _Account, value);
        }

        #endregion



        #region То что относится к серверу

        public static InstanceContext context = new InstanceContext(new MyCallback());
        public static DuplexChannelFactory<IChatService> factory;
        public static IChatService server;
        NetTcpBinding netTcpBinding = new NetTcpBinding();

        #endregion








    }

    class MyCallback : IChatClient
    {
        public void RecievAudio(string user, byte[] message)
        {
            throw new NotImplementedException();
        }

        public void RecievMessage(string user, string message)
        {
            throw new NotImplementedException();
        }
    }
}
