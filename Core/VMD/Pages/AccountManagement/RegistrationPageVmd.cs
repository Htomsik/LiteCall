using System.Collections.ObjectModel;
using System.Windows.Input;
using Core.Infrastructure.CMD;
using Core.Models.AccountManagement;
using Core.Models.Users;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Extra;
using Core.VMD.Base;
using ReactiveUI;

namespace Core.VMD.Pages.AccountManagement;

public class RegistrationPageVmd : BaseVmd
{
    private readonly IGetCaptchaSc _getCaptchaSc;

    private readonly IEncryptSc _encryptSc;

    private readonly IGetRecoveryQuestionsSc _getRecoveryQuestionsSc;


    private readonly IRegistrationSc _registrationSc;


    private bool _canServerConnect;


    private string? _captchaString;


    private byte[]? _captchaBytes;


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
        
        RegistrationCommand = ReactiveCommand.CreateFromTask(OnRegistrationExecuted,this.WhenAnyValue(x=>x.CaptchaString,(captchaString)=>!string.IsNullOrEmpty(captchaString)));
        
        OpenModalCommand = ReactiveCommand.CreateFromTask(OnOpenModalCommandExecuted, CanOpenModalCommandExecute());
        
        OpenAuthPageCommand = new NavigationCommand(authPageNavigationScs);
        
     
        
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

    public byte[]? CaptchaBytes
    {
        get => _captchaBytes;
        set => this.RaiseAndSetIfChanged(ref _captchaBytes, value);
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

    public IReactiveCommand RegistrationCommand { get; set; }
    
    private async Task OnRegistrationExecuted()
    {
        var newAccount = new Account
        {
            Login = Login
        };
        
        try
        {
            var base64ShaPassword = await _encryptSc.ShaEncrypt(Password);

            base64ShaPassword = await _encryptSc.Base64Encrypt(base64ShaPassword);

            Password = base64ShaPassword;
            
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
        catch (Exception)
        {
            //ignored
        }
        
       
    }


    public IReactiveCommand OpenModalCommand { get; }

    private async Task OnOpenModalCommandExecuted()
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

    
    private IObservable<bool> CanOpenModalCommandExecute() =>
        this.WhenAnyValue(x => x.Login, x => x.Password,
            x => x.QuestionAnswer,
            x => x.SelectedQuestion,x=>x.ConfirmPassword,
            (login, password, questionAnswer, selectedQuestion,confirmPassword) =>
            {
                if (Password != confirmPassword) return false;

                return !string.IsNullOrEmpty(Login) && !string.IsNullOrEmpty(Password) &&
                       !string.IsNullOrEmpty(confirmPassword) && !string.IsNullOrEmpty(QuestionAnswer) &&
                       SelectedQuestion is not null;
            });
              


    public async Task<bool> GetCaptcha()
    {
        CaptchaBytes = await _getCaptchaSc.GetCaptcha();

        return CaptchaBytes != null;
    }


    public ICommand OpenAuthPageCommand { get; }

    #endregion
}