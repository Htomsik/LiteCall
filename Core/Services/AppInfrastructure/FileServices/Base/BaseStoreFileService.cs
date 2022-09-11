using AppInfrastructure.Stores.DefaultStore;
using Newtonsoft.Json;

namespace Core.Services.AppInfrastructure.FileServices.Base;

public abstract class BaseStoreFileService<TValue> : IFileService
{
    #region Stores

    private readonly IStore<TValue?> _store;

    #endregion
    
    #region Properties and Fields

    protected abstract string Path { get; set; }

    #endregion
    
    #region Constructors

    public BaseStoreFileService(IStore<TValue?> store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));

        _store.CurrentValueChangedNotifier += SaveDataInFile;
    } 
    
    #endregion
    
    #region Methods
    
    #region GetDataFromFile

    public async void GetDataFromFile()
    {
        string textFromFile;

        using (StreamReader reader = new StreamReader(Path))
        {
            textFromFile = await reader.ReadToEndAsync();
        }
        
        if (string.IsNullOrEmpty(textFromFile))
            //Add loger later
            return;
        
        TValue? deserializedValue = default;
        
        try
        {
            deserializedValue = JsonConvert.DeserializeObject<TValue>(textFromFile);
        }
        catch (Exception e)
        {
            // Add logger later
        }
        
        if (deserializedValue is null || deserializedValue.Equals(default(TValue)))
            //Add logger later
            return;
        
        _store.CurrentValue = deserializedValue;
    }

    #endregion

    #region SaveDataInFile

    public async void SaveDataInFile()
    {
        TValue? valueIntoFile = _store.CurrentValue;

        if (valueIntoFile is null)
            return;

        string seriaLizedValue = default;
        
        try
        {
            seriaLizedValue = JsonConvert.SerializeObject(valueIntoFile);
        }
        catch (Exception e)
        {
            // Add logger later
        }
        
        using (StreamWriter writer = new StreamWriter(Path, false))
        {
            await writer.WriteLineAsync(seriaLizedValue);
        }
    }

    #endregion
    
    #endregion
    
}