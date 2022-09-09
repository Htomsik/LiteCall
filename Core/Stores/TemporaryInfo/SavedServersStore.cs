using System.Collections.ObjectModel;
using AppInfrastructure.Stores.Repositories.Collection;
using Core.Models.Saved;
using Core.Models.Servers;


namespace Core.Stores.TemporaryInfo;

public sealed class SavedServersStore : BaseLazyCollectionRepository<ObservableCollection<ServerAccount>,ServerAccount>
{
    
    public bool Replace(ServerAccount? serverAccount)
    {
        if (serverAccount is null || serverAccount.Account is null || serverAccount.SavedServer is null)
            throw new ArgumentNullException(nameof(serverAccount));
       
        var  isReplaced = false;
        
        var foundAcc = FIndByServerApiIp(serverAccount.SavedServer);
        
        if (Contains(foundAcc) && removeFromEnumerable(foundAcc))
        {
            addIntoEnumerable(serverAccount);
            isReplaced = true;
        }
        
        OnCurrentValueChanged();
        return isReplaced;
    }
    
    public ServerAccount FIndByServerApiIp(Server server) => 
        server is null  
        ? default
        : CurrentValue?.FirstOrDefault(item=>item.SavedServer.ApiIp == server.ApiIp,default);

    public bool ContainsByServerApiIp(Server server) => 
        server is null 
            ? false
            : !FIndByServerApiIp(server).Equals(default);

}