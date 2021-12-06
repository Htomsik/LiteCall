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
        [OperationContract(IsOneWay = true)]
        void Join(string username);
        
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


    }
    [ServiceContract]
    public interface IChatClient
    {
        [OperationContract(IsOneWay = true)]
        void RecievAudio(string user, byte[] message);
       
        [OperationContract(IsOneWay = true)]
        void RecievMessage(string user, string message);
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
