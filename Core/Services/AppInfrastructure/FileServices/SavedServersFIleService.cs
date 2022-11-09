using System.Collections.ObjectModel;
using Core.Models.Saved;
using Core.Services.AppInfrastructure.FileServices.Base;
using Core.Stores.TemporaryInfo;

namespace Core.Services.AppInfrastructure.FileServices;

/// <summary>
///     File Service for saved servers
/// </summary>
public class SavedServersFIleService : DataFileService<ObservableCollection<ServerAccount>>
{
    public SavedServersFIleService(MainAccountStore mainAccountStore, SavedServersStore store) : base(mainAccountStore, store)
    {
    }

    protected override string FileName => "SavedServers.json";
}