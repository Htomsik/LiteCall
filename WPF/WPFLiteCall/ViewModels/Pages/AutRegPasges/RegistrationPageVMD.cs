using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Core.Infrastructure.CMD;
using Core.Infrastructure.CMD.Lambda;
using Core.Models;
using Core.Models.AccountManagement;
using Core.Models.Users;
using Core.VMD.Base;
using Core.Services;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Extra;
using LiteCall.Services.Interfaces;
using ReactiveUI;

namespace LiteCall.ViewModels.Pages.AutRegPasges;

internal class RegistrationPageVmd : BaseVmd
{
    private readonly IGetCaptchaSc _getCaptchaSc;

    private readonly IEncryptSc _encryptSc;

    private readonly IGetRecoveryQuestionsSc _getRecoveryQuestionsSc;


    private readonly IRegistrationSc _registrationSc;


    private bool _canServerConnect;


    private string? _captchaString;


    private ImageSource? _captсhaImageSource;


    private string? _confirmPassword;


    private string? _login;


    private bool _modalStatus;


    private string? _password;


    private string? _questionAnswer;


    private ObservableCollection<Question>? _questionsCollection;


    private Question? _selectedQuestion;

    public RegistrationPageVmd(INavigationSc authPageNavigationScs,
        IRegistrationSc registrationSc, IStatusSc statusSc,
        IGetCaptchaSc getCaptchaSc, IGetRecoveryQuestionsSc getRecoveryQuestionsSc,
        IEncryptSc encryptSc)
    {
        _registrationSc = registrationSc;


        _getCaptchaSc = getCaptchaSc;

        _getRecoveryQuestionsSc = getRecoveryQuestionsSc;

        _encryptSc = encryptSc;


        RegistrationCommand = new AsyncLambdaCmd(OnRegistrationExecuted,
            ex => statusSc.ChangeStatus(ex.Message),
            CanRegistrationExecute);

        OpenAuthPageCommand = new NavigationCommand(authPageNavigationScs);

        OpenModalCommand = new AsyncLambdaCmd(OnOpenModalCommandExecuted,
            ex => statusSc.ChangeStatus(ex.Message),
            CanOpenModalCommandExecute);


        GetQuestionList();
    }

    public bool CanServerConnect
    {
        get => _canServerConnect;
        set => this.RaiseAndSetIfChanged(ref _canServerConnect, value);
    }

    public bool ModalStatus
    {
        get => _modalStatus;
        set => this.RaiseAndSetIfChanged(ref _modalStatus, value);
    }

    public string? CaptchaString
    {
        get => _captchaString;
        set => this.RaiseAndSetIfChanged(ref _captchaString, value);
    }

    public ImageSource? CaptсhaImageSource
    {
        get => _captсhaImageSource;
        set => this.RaiseAndSetIfChanged(ref _captсhaImageSource, value);
    }

    public string? Login
    {
        get => _login;
        set => this.RaiseAndSetIfChanged(ref _login, value);
    }

    public string? Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string? ConfirmPassword
    {
        get => _confirmPassword;
        set => this.RaiseAndSetIfChanged(ref _confirmPassword, value);
    }

    public string? QuestionAnswer
    {
        get => _questionAnswer;
        set => this.RaiseAndSetIfChanged(ref _questionAnswer, value);
    }

    public ObservableCollection<Question>? QuestionsCollection
    {
        get => _questionsCollection;
        set => this.RaiseAndSetIfChanged(ref _questionsCollection, value);
    }

    public Question? SelectedQuestion
    {
        get => _selectedQuestion;
        set => this.RaiseAndSetIfChanged(ref _selectedQuestion, value);
    }


    private async void GetQuestionList()
    {
        try
        {
            QuestionsCollection =
                new ObservableCollection<Question>((await _getRecoveryQuestionsSc.GetQuestions())!);

            CanServerConnect = true;
        }
        catch
        {
            CanServerConnect = false;

            OpenAuthPageCommand.Execute(null);
        }
    }

    #region Commands

    public ICommand RegistrationCommand { get; set; }

    private bool CanRegistrationExecute(object p)
    {
        return !(bool)p && !string.IsNullOrEmpty(CaptchaString);
    }

    private async Task OnRegistrationExecuted(object p)
    {
        var base64Sha1Password = await _encryptSc.Sha1Encrypt(Password);

        base64Sha1Password = await _encryptSc.Base64Encrypt(base64Sha1Password);

        var newAccount = new Account
        {
            Login = Login,
            Password = base64Sha1Password
        };

        var registrationModel = new RegistrationModel
        {
            Captcha = CaptchaString,
            QuestionAnswer = QuestionAnswer,
            Question = SelectedQuestion,
            RecoveryAccount = newAccount
        };

        var response = await _registrationSc.Registration(registrationModel);

        switch (response)
        {
            case 0:
                await GetCaptcha();
                break;

            case 1:
                ModalStatus = false;

                break;
        }
    }


    public ICommand OpenModalCommand { get; }

    private async Task OnOpenModalCommandExecuted(object p)
    {
        if (ModalStatus == false)
        {
            var status = await GetCaptcha();

            if (!status) return;

            ModalStatus = true;
        }
        else
        {
            ModalStatus = false;

            CaptchaString = string.Empty;
        }
    }


    private bool CanOpenModalCommandExecute(object p)
    {
        if (Password != ConfirmPassword) return false;

        return !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password) &&
               !string.IsNullOrEmpty(ConfirmPassword) && !string.IsNullOrEmpty(QuestionAnswer) &&
               SelectedQuestion is not null;
    }


    public async Task<bool> GetCaptcha()
    {
        CaptсhaImageSource = await _getCaptchaSc.GetCaptcha();

        return CaptсhaImageSource != null;
    }


    public ICommand OpenAuthPageCommand { get; }

    #endregion
}