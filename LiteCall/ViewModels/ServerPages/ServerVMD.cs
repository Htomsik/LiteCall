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
            factory = new DuplexChannelFactory<IChatService>(context, netTcpBinding, "net.tcp://51.12.240.57:7998/WPFChat");
            server = factory.CreateChannel();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://51.12.240.57:7997/WPFHost/");

            request.Timeout = 1000;

            try
            {
                request.GetResponse();
                test = 1;
            }
            catch (Exception e)
            {
                test = 0;
            }

           




        }

        private Account _Account;

        public Account Account
        {
            get => _Account;
            set => Set(ref _Account, value);
        }


        public static InstanceContext context = new InstanceContext(new MyCallback());
        public static DuplexChannelFactory<IChatService> factory;
        public static IChatService server;
        NetTcpBinding netTcpBinding = new NetTcpBinding();


        public ObservableCollection<ServerUser> Users { get; }



        private int _test;

        public int test
        {
            get => _test;
            set => Set(ref _test, value);
        }
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
