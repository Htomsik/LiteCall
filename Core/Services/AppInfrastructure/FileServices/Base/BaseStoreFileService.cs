using AppInfrastructure.Stores.DefaultStore;
using Core.Services.Extensions;
using Newtonsoft.Json;

namespace Core.Services.AppInfrastructure.FileServices.Base;

/// <summary>
///     Base realiizing for IFileService. Saving on Json format
/// </summary>
/// <typeparam name="TValue">Some generic value</typeparam>
public abstract class BaseStoreFileService<TValue> : IFileService
{
    #region Stores

    protected readonly IStore<TValue?> Store;

    #endregion
    
    #region Properties and Fields

    #region DirectoryPath
    
    /// <summary>
    ///     Path to directory with file
    /// </summary>
    protected abstract string DirectoryPath { get; set; }
    
    /// <summary>
    ///     Json file name.
    /// </summary>
    protected abstract string FileName { get; }

    #endregion
    
    #endregion
    
    #region Constructors

    public BaseStoreFileService(IStore<TValue?> store)
    {
        Store = store ?? throw new ArgumentNullException(nameof(store));

        Store.CurrentValueChangedNotifier += SaveDataInFile;
        
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
        
        Store.CurrentValue = deserializedValue;
    }

    #endregion
    
    #region SaveDataInFile

    public async void SaveDataInFile()
    {
        if (!FileExtensions.IsFileExist(FileName, DirectoryPath))
            FileExtensions.RestoreFile(FileName, DirectoryPath);
        
        TValue? valueIntoFile = Store.CurrentValue;

        if (valueIntoFile is null)
            return;

        string seriaLizedValue = default;
        
        try
        {
            seriaLizedValue = JsonConvert.SerializeObject(valueIntoFile,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                } );
        }
        catch (Exception e)
        {
            // Add logger later
        }
        
        using (StreamWriter writer = new StreamWriter($"{DirectoryPath}/{FileName}", false))
        {
            await writer.WriteLineAsync(seriaLizedValue);
        }
    }

    #endregion
    
    #endregion

}