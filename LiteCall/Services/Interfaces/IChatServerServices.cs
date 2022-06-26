using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Model.ServerModels.Messages;

namespace LiteCall.Services.Interfaces
{
    internal interface IChatServerServices
    {
        public  Task<bool> GroupConnect(string roomName, string roomPassword);
        public  Task<bool> GroupDisconnect();
        public  Task<bool> GroupCreate(string roomName, string roomPassword);
        public  Task<bool> AdminDeleteGroup(string roomName);
        public  Task<bool> AdminKickUserFromGroup(string userName);
        public  Task<bool> SendMessage(Message newMessage);
        public  Task ConnectionStop();
        public  Task SendAudioMessage(byte[] audioBuffer);
    }
}
