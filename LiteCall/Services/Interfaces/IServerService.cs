using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace AudioLib
{

    [ServiceContract(CallbackContract = typeof(IChatClient))]
    public interface IChatService
    {
        [OperationContract]
        bool Join(string username, string password, bool reg);

        [OperationContract(IsOneWay = true)]
        void Disconect();
        [OperationContract(IsOneWay = true)]
        void SendMessage(string message,string name);
        [OperationContract(IsOneWay = true)]
        void SendAudo(byte[] audio, string name);
        [OperationContract]
        List<string> GetUserList();
        [OperationContract]
        List<string> GetRoomList();
        [OperationContract]
        string CreateRoom(string name);

        [OperationContract]
        List<string> ServerInfo();


    }
    [ServiceContract]
    public interface IChatClient
    {
        [OperationContract(IsOneWay = true)]
        void RecievAudio(string user, byte[] message, bool type);

        [OperationContract(IsOneWay = true)]
        void RecievMessage(string user, string message, bool type);
        [OperationContract(IsOneWay = true)]
        void Refresh();
    }

    [ServiceContract(CallbackContract = typeof(ICreateRoom))]
    public interface ICreateRoomService
    {

        [OperationContract]
        void ServerJoin(string username);
    }
    
    [ServiceContract]
    public interface ICreateRoom
    {
        [OperationContract]
        string CreatRoomCallback(string name);
    }

    //public interface IChatClientMessage
    //{
    //    [OperationContract(IsOneWay = true)]
    //    void RecievMessage(string user, string message);
    //}
}
