using System.Threading.Tasks;

namespace LiteCall.Services.Interfaces;

public interface ISyncDataOnServerSc
{
    public Task<bool> SaveOnServer();

    public Task<bool> GetFromServer();
}