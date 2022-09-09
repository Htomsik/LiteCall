using Core.Models.Saved;
using Core.Models.Servers;
using Core.Models.Users;
using Core.Stores.TemporaryInfo;

namespace CoreTests.Stores;

[TestClass]
public class SavedServersStoreUnitTests
{
    [TestMethod]
    public void AddSameServer()
    {
        //Arrange
        var savedServersStore = new SavedServersStore();

        var addingServer = new ServerAccount { 
            SavedServer = new Server{ApiIp = "TestApiIp",Title = "TestTitle", Ip = "TestIp"},
            Account = new Account{CurrentServerLogin = "TestServerLogin", IsAuthorized = false, Login = "TestLogin"}
        };
        
        //Act
        savedServersStore.AddIntoEnumerable(addingServer);
        
        //Act+Assert
        Assert.ThrowsException<Exception>(() => savedServersStore.AddIntoEnumerable(addingServer));

    }

    [TestMethod]
    public void RemoveDoesntExistServer()
    {
        //Arrange
        var savedServersStore = new SavedServersStore();

        var removedServer = new ServerAccount { 
            SavedServer = new Server{ApiIp = "TestApiIp",Title = "TestTitle", Ip = "TestIp"},
            Account = new Account{CurrentServerLogin = "TestServerLogin", IsAuthorized = false, Login = "TestLogin"}
        };
        
        //Act+Assert
        Assert.ThrowsException<Exception>(() => savedServersStore.RemoveFromEnumerable(removedServer));
    }
}