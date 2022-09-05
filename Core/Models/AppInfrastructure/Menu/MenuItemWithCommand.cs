using System.Windows.Input;

namespace Core.Models.AppInfrastructure.Menu;

public class MenuItemWithCommand: MenuItem
{
    /// <summary>
    ///     Command pointer
    /// </summary>
    public Lazy<ICommand> Command { get; }
    
    /// <summary>
    ///     Command parameter
    /// </summary>
    public Lazy<object> Parameter { get; }

 
    /// <param name="name">Element name</param>
    /// <param name="command">Icommand command</param>
    /// <param name="parameter">Command parameter</param>
    /// <exception cref="ArgumentNullException">If command or name is null</exception>
    public MenuItemWithCommand(string name, ICommand command, object? parameter = null) : base(name)
    {
        Command = command is null 
            ? throw  new ArgumentNullException(nameof(command))
            : new Lazy<ICommand>(()=> command);
        
        Parameter = new Lazy<object>(()=> parameter);
    }
}