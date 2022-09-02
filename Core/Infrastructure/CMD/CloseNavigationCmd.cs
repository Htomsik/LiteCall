using AppInfrastructure.Services.NavigationServices.Close;
using Core.Infrastructure.CMD.Base;

namespace Core.Infrastructure.CMD;

/// <summary>
///     Навигационная команда для закрытия
/// </summary>
internal sealed class CloseNavigationCmd : BaseCmd
{
    private readonly Lazy<Predicate<object>>  _canExecute;
    private readonly Lazy<ICloseServices>  _closeNavigationServices;

    /// <summary>
    ///  Контруктор для условий выполнения с входным и выходным параметром
    /// </summary>
    /// <param name="closeNavigationServices">Сервис закрытия</param>
    /// <param name="canExecute">Условие выполнения команды</param>
    /// <exception cref="ArgumentNullException">Возникает в случае если _closeNavigationServices null</exception>
    public CloseNavigationCmd(ICloseServices closeNavigationServices, Predicate<object> canExecute = null)
    {
        _closeNavigationServices =  closeNavigationServices is null
                ? throw new ArgumentNullException(nameof(_closeNavigationServices)) :
            new Lazy<ICloseServices>(()=>closeNavigationServices);

        _canExecute = new Lazy<Predicate<object>>(()=>canExecute);
    }

    /// <summary>
    ///  Контруктор для условий выполнения только с выходным параметром
    /// </summary>
    /// <param name="closeNavigationServices">Сервис закрытия</param>
    /// <param name="canExecute">Условие выполнения команды</param>
    /// <exception cref="ArgumentNullException">Возникает в случае если _closeNavigationServices null</exception>
    public CloseNavigationCmd(ICloseServices closeNavigationServices, Func<bool> canExecute = null)
        : this(
            closeNavigationServices ??
            throw new ArgumentNullException(nameof(_closeNavigationServices))
            , canExecute is null ? null : p => canExecute())
    {
    }
    
    /// <summary>
    ///  Контруктор без условия выполнения
    /// </summary>
    /// <param name="closeNavigationServices">Сервис закрытия</param>
    /// <exception cref="ArgumentNullException">Возникает в случае если _closeNavigationServices null</exception>
    public CloseNavigationCmd(ICloseServices closeNavigationServices) 
        : this(closeNavigationServices 
               ?? throw new ArgumentNullException(nameof(_closeNavigationServices)),
        p => true)
    {
    }

    public override void Execute(object? parameter) => _closeNavigationServices.Value.Close();
    
    public override bool CanExecute(object parameter) => _canExecute.Value(parameter);

}