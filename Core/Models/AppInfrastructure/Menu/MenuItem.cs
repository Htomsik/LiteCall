namespace Core.Models.AppInfrastructure.Menu;

/// <summary>
///     Menu element
/// </summary>
public class MenuItem
{
    public Lazy<string>  Name { get; }
    
    /// <param name="name">Element Name</param>
    public MenuItem(string name)
    {
        Name = name is null 
            ? throw  new ArgumentNullException(nameof(name))
            : new Lazy<string>(()=> name);

    }
}