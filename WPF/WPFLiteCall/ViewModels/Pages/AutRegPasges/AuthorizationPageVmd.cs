using System.Threading.Tasks;
using System.Windows.Input;
using Core.Infrastructure.CMD;
using Core.Infrastructure.CMD.Lambda;
using Core.Models.Users;
using Core.VMD.Base;
using Core.Services;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Extra;
using LiteCall.Services.Interfaces;

namespace LiteCall.ViewModels.Pages.AutRegPasges;

internal class AuthorizationPageVmd : BaseVmd
{
    private readonly IEncryptSc _encryptSc;

    public AuthorizationPageVmd(INavigationSc registrationNavigationScs,
        INavigationSc passwordRecoveryNavigationSc, IAuthorizationSc authorizationSc,
        IEncryptSc encryptSc)
    {
        _encryptSc = encryptSc;

        _authorizationSc = authorizationSc;

        AuthCommand =
            new AsyncLambdaCmd(OnAuthExecuteExecuted, ex => StatusMessage = ex.Message, CanAuthExecute);

        OpenRegistrationPageCommand = new NavigationCommand(registrationNavigationScs);

        OpenRecoveryPasswordPageCommand = new NavigationCommand(passwordRecoveryNavigationSc);
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
        var base64Sha1Password = await _encryptSc.Sha1Encrypt(Password);

        base64Sha1Password = await _encryptSc.Base64Encrypt(base64Sha1Password);

        var newAccount = new Account
        {
            Login = Login,
            Password = base64Sha1Password
        };

        var response = await _authorizationSc.Login(!CheckStatus, newAccount);

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

    private readonly IAuthorizationSc _authorizationSc;


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