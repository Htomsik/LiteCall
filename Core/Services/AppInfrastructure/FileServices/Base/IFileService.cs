namespace Core.Services.AppInfrastructure.FileServices.Base;

/// <summary>
///     FIleService for saving data into local stores (xml, JSon etc) files
/// </summary>
public interface IFileService
{
    
    void GetDataFromFile();

    void SaveDataInFile();
}