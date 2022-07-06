using Core.Models.Servers.Messages;

namespace Core.Services.Interfaces.Connections;

public interface IChatServerSc
{
    public Task<bool> GroupConnect(string roomName, string roomPassword);
    public Task<bool> GroupDisconnect();
    public Task<bool> GroupCreate(string roomName, string roomPassword);
    public Task<bool> AdminDeleteGroup(string roomName);
    public Task<bool> AdminKickUserFromGroup(string userName);
    public Task<bool> SendMessage(TextMessage newMessage);
    public Task ConnectionStop();
    public Task SendAudioMessage(byte[] audioBuffer);
}