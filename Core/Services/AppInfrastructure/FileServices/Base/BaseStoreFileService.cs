using AppInfrastructure.Stores.DefaultStore;
using Core.Services.Extensions;
using Newtonsoft.Json;

namespace Core.Services.AppInfrastructure.FileServices.Base;

public abstract class BaseStoreFileService<TValue> : IFileService
{
    #region Stores

    protected readonly IStore<TValue?> _store;

    #endregion
    
    #region Properties and Fields

    #region DirectoryPath
    
    /// <summary>
    ///     File filePath for child
    /// </summary>
    protected abstract string DirectoryPath { get; set; }
    
    protected abstract string FileName { get; }

    #endregion
    
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
    public void GetDataFromFile()
    {
        if (!FileExtensions.IsFileExist(FileName, DirectoryPath))
        {
            FileExtensions.RestoreFile(FileName, DirectoryPath);
            SaveDataInFile();
            return;
        }
        
        string textFromFile;
        
        using (StreamReader reader = new StreamReader($"{DirectoryPath}/{FileName}"))
        {
            textFromFile = reader.ReadToEnd();
            Task.WaitAll();
        }

        if (string.IsNullOrEmpty(textFromFile))
        {
            SaveDataInFile();
            //Add logger later
            return;
        }
        
        
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
        if (!FileExtensions.IsDirectoryExist(DirectoryPath) || !FileExtensions.RestoreDirectories(DirectoryPath))
            // Add logger later
            return;
        
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
        
        using (StreamWriter writer = new StreamWriter($"{DirectoryPath}/{FileName}", false))
        {
            await writer.WriteLineAsync(seriaLizedValue);
            Task.WaitAll();
        }

       
    }

    #endregion
    
    #endregion

}