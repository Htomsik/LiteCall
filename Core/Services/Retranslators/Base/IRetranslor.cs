namespace Core.Services.Retranslators.Base;

/// <summary>
///     Retraslator bewtwen nonConnetions modules
/// </summary>
/// <typeparam name="TInput">Input parameter</typeparam>
/// <typeparam name="TOutput">Output parameter</typeparam>
public interface IRetranslor<TInput,TOutput>
{
    TOutput Retranslate(TInput parameter);
}