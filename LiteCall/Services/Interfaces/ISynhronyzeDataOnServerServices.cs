using System.Threading.Tasks;

namespace LiteCall.Services.Interfaces;

internal interface ISynhronyzeDataOnServerServices
{
    public Task<bool> SaveOnServer();

    public Task<bool> GetFromServer();
}