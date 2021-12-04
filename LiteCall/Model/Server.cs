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
        private IPAddress _IP;
        /// <summary>
        /// Ip сервера
        /// </summary>
        public IPAddress IP
        {
            get => _IP;
            set => Set(ref _IP, value);
        }

        private string _Name;
        /// <summary>
        /// Имя сервера
        /// </summary>
        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }

        private int _MaxUsers;
        /// <summary>
        /// Максимальное еоличество пользователей сервера
        /// </summary>
        public int MaxUsers
        {
            get => _MaxUsers;
            set => Set(ref _MaxUsers, value);
        }

        private int _CurrentUsers;
        /// <summary>
        /// Сколько пользователей на данный момент
        /// </summary>
        public int CurrentUsers
        {
            get => _CurrentUsers;
            set => Set(ref _CurrentUsers, value);
        }

        private ICollection<ServerRooms> _Rooms;
        /// <summary>
        /// Комнаты на сервере
        /// </summary>
        public ICollection<ServerRooms> Rooms
        {
            get => _Rooms;
            set => Set(ref _Rooms, value);
        }


        private bool _Status;
        /// <summary>
        /// Статус сервера (вкл/выкл)
        /// </summary>
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


        private ICollection<User> _Users;

        public ICollection<User> Users
        {
            get => _Users;
            set => Set(ref _Users, value);
        }


    }
}
