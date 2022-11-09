using System.Windows.Input;
using AppInfrastructure.Services.NavigationServices.Navigation;
using Core.Infrastructure.CMD;
using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.Extra;
using Core.VMD.Base;
using ReactiveUI;

namespace Core.VMD.Pages.AccountManagement;

public class AuthorizationPageVmd : BaseVmd
{
    private readonly IEncryptSc _encryptSc;

    public AuthorizationPageVmd(INavigationServices registrationNavigationServiceses,
        INavigationServices passwordRecoveryNavigationServices, IAuthorizationSc authorizationSc,
        IEncryptSc encryptSc)
    {
        _encryptSc = encryptSc;

        _authorizationSc = authorizationSc;
        
        AuthCommand = ReactiveCommand.CreateFromTask(OnAuthExecuted, CanAuthExecute);

        OpenRegistrationPageCommand = new NavigationCommand(registrationNavigationServiceses);

        OpenRecoveryPasswordPageCommand = new NavigationCommand(passwordRecoveryNavigationServices);
    }
    
    #region Команды

    public ICommand AuthCommand { get; }

    
    private IObservable<bool> CanAuthExecute => this.WhenAnyValue(x => x.CheckStatus, x => x.Login, x => x.Password,
        (checkStatus, login, password) =>
        {
            if (CheckStatus && !string.IsNullOrEmpty(Login)) return true;
            return !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password);
        });

    private async Task OnAuthExecuted()
    {
        var newAccount = new Account
        {
            Login = Login
        };
        
        try
        {
            if (!_checkStatus)
            {
                var base64ShaPassword = await _encryptSc.ShaEncrypt(Password);

                base64ShaPassword = await _encryptSc.Base64Encrypt(base64ShaPassword);

                newAccount.Password = base64ShaPassword;
            }
            
            await _authorizationSc.Login(!CheckStatus, newAccount);
                
        }
        catch (Exception )
        {
            StatusMessage = null;
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
        set => this.RaiseAndSetIfChanged(ref _checkStatus, value);
    }


    private string? _login;

    public string? Login
    {
        get => _login;
        set => this.RaiseAndSetIfChanged(ref _login, value);
    }


    private string? _password;

    public string? Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }


    private string? _statusMessage;

    public string? StatusMessage
    {
        get => _statusMessage;
        set
        {
            this.RaiseAndSetIfChanged(ref _statusMessage, value);
            this.RaisePropertyChanged(nameof(HasStatusMessage));
        }
    }

    public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

    #endregion
}