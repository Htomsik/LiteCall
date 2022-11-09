namespace Core.Services.Extensions;

/// <summary>
///     Extensions fore file i/O
/// </summary>
public static class FileExtensions
{
    #region RestoreDirectories :  Restore directory path

    /// <summary>
    ///     Restore directory path
    /// </summary>
    /// <param name="path">Path to directory</param>
    /// <returns>True if path restored</returns>
    /// <exception cref="ArgumentNullException">If path is null</exception>
    public static bool RestoreDirectories(string path)
    {
        if (IsDirectoryExist(path))
            return true;

        try
        {
            Directory.CreateDirectory(path);
        }
        catch (Exception e)
        {
            //Add logging later
            return false;
        }

        return true;
    }

    #endregion

    #region IsDirectoryExist :  Check of existing directories

    /// <summary>
    ///     Check of existing directories
    /// </summary>
    /// <param name="path">Path to directory</param>
    /// <returns>True if path existing</returns>
    /// <exception cref="ArgumentNullException">If path is null</exception>
    public static bool IsDirectoryExist(string path)
    {
        return string.IsNullOrEmpty(path.Trim())
            ? throw new ArgumentNullException(nameof(path))
            : Directory.Exists(path);
    }

    #endregion

    #region RestoreFile : Restore file in Data/{DirectoryPath}/

    /// <summary>
    ///     Restore file in Data/{DirectoryPath}/
    /// </summary>
    /// <param name="fileName">File in directory</param>
    /// <param name="directoryPath">Directory path</param>
    /// <exception cref="ArgumentNullException">If fileName or directoryPath is null</exception>
    /// <returns>True if restored</returns>
    public static bool RestoreFile(string fileName, string directoryPath)
    {
        if (IsFileExist(directoryPath, fileName))
            return true;

        if (!IsDirectoryExist(directoryPath))
            RestoreDirectories(directoryPath);

        try
        {
            File.Create($"{directoryPath}/{fileName}").Close();
            
        }
        catch (Exception e)
        {
            //Add logging later
            return false;
        }

        return true;
    }

    #endregion

    #region IsFileExist : Check of existing files

    /// <summary>
    ///     Check of existing files
    /// </summary>
    /// <param name="fileName">File in directory</param>
    /// <param name="directoryPath">Directory path</param>
    /// <returns>True if file existiing</returns>
    /// <exception cref="ArgumentNullException">If fileName or directoryPath is null</exception>
    public static bool IsFileExist(string fileName, string directoryPath)
    {
        directoryPath = string.IsNullOrEmpty(directoryPath.Trim())
            ? throw new ArgumentNullException(nameof(directoryPath))
            : directoryPath;

        fileName = string.IsNullOrEmpty(directoryPath.Trim())
            ? throw new ArgumentNullException(nameof(fileName))
            : fileName;

        return File.Exists($"{directoryPath}/{fileName}");
    }

    #endregion
}