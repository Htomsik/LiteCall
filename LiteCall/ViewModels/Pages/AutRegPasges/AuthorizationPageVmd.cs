using System.Threading.Tasks;
using System.Windows.Input;
using LiteCall.Infrastructure.Commands;
using LiteCall.Infrastructure.Commands.Lambda;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.ViewModels.Base;

namespace LiteCall.ViewModels.Pages;

internal class AuthorizationPageVmd : BaseVmd
{
    private readonly IEncryptServices _encryptServices;

    public AuthorizationPageVmd(INavigationService registrationNavigationServices,
        INavigationService passwordRecoveryNavigationService, IAuthorizationServices authorizationServices,
        IEncryptServices encryptServices)
    {
        _encryptServices = encryptServices;

        _authorizationServices = authorizationServices;

        AuthCommand =
            new AsyncLambdaCommand(OnAuthExecuteExecuted, ex => StatusMessage = ex.Message, CanAuthExecute);

        OpenRegistrationPageCommand = new NavigationCommand(registrationNavigationServices);

        OpenRecoveryPasswordPageCommand = new NavigationCommand(passwordRecoveryNavigationService);
    }


    #region Команды

    public ICommand AuthCommand { get; }

    private bool CanAuthExecute(object p)
    {
        if (CheckStatus && !string.IsNullOrEmpty(Login)) return true;
        return !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password);
    }

    private async Task OnAuthExecuteExecuted(object p)
    {
        var base64Sha1Password = await _encryptServices.Sha1Encrypt(Password);

        base64Sha1Password = await _encryptServices.Base64Encrypt(base64Sha1Password);

        var newAccount = new Account
        {
            Login = Login,
            Password = base64Sha1Password
        };

        var response = await _authorizationServices.Login(!CheckStatus, newAccount);

        switch (response)
        {
            case 0:
                break;

            case 1:
                StatusMessage = null;
                break;
        }
    }

    public ICommand OpenRegistrationPageCommand { get; }

    public ICommand OpenRecoveryPasswordPageCommand { get; }

    #endregion

    #region Данные с формы

    private readonly IAuthorizationServices _authorizationServices;


    private bool _checkStatus;

    public bool CheckStatus
    {
        get => _checkStatus;
        set => Set(ref _checkStatus, value);
    }


    private string? _login;

    public string? Login
    {
        get => _login;
        set => Set(ref _login, value);
    }


    private string? _password;

    public string? Password
    {
        get => _password;
        set => Set(ref _password, value);
    }


    private string? _statusMessage;

    public string? StatusMessage
    {
        get => _statusMessage;
        set
        {
            Set(ref _statusMessage, value);
            OnPropertyChanged(nameof(HasStatusMessage));
        }
    }

    public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

    #endregion
}