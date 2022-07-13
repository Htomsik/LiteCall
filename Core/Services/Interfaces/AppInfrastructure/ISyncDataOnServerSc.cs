namespace Core.Services.Interfaces.AppInfrastructure;

public interface ISyncDataOnServerSc
{
    public Task<bool> SaveOnServer();

    public Task<bool> GetFromServer();
}