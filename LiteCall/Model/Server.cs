using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LiteCall.ViewModels.Base;

namespace LiteCall.Model
{
    internal class Server:BaseVMD
    {
        private string _IP;
        public string IP
        {
            get => _IP;
            set => Set(ref _IP, value);
        }

        private string _Name;
        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }

    

        private int _CurrentUsers;
        public int CurrentUsers
        {
            get => _CurrentUsers;
            set => Set(ref _CurrentUsers, value);
        }


        private int _MaxUsers;

        public int MaxUsers
        {
            get => _MaxUsers;
            set => Set(ref _MaxUsers, value);
        }


        private bool _Status;
        public bool Status
        {
            get => _Status;
            set => Set(ref _Status, value);
        }
    }

    internal class ServerRooms : BaseVMD
    {

        private string _Name;
        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }


        private ICollection<ServerUser> _Users;

        public ICollection<ServerUser> Users
        {
            get => _Users;
            set => Set(ref _Users, value);
        }


    }
}
