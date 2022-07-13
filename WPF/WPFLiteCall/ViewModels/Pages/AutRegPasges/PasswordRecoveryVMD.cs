using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Core.Infrastructure.CMD;
using Core.Infrastructure.CMD.Lambda;
using Core.Models;
using Core.Models.AccountManagement;
using Core.Models.Users;
using Core.Services;
using Core.Services.Interfaces.AccountManagement;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Services.Interfaces.Extra;
using Core.VMD.Base;
using LiteCall.Services.Interfaces;
using ReactiveUI;

namespace LiteCall.ViewModels.Pages.AutRegPasges;

internal class PasswordRecoveryVmd : BaseVmd
{
    private readonly IEncryptSc _encryptSc;
    private readonly IGetRecoveryQuestionsSc _getRecoveryQuestionsSc;

    private readonly IRecoveryPasswordSc _recoveryPasswordSc;

    public PasswordRecoveryVmd(INavigationSc authPageNavigationScs, IStatusSc statusSc,
        IGetRecoveryQuestionsSc getRecoveryQuestionsSc,
        IRecoveryPasswordSc recoveryPasswordSc, IEncryptSc encryptSc)
    {
        _getRecoveryQuestionsSc = getRecoveryQuestionsSc;

        _recoveryPasswordSc = recoveryPasswordSc;

        _encryptSc = encryptSc;


        #region Команды

        #region Асинхронные

        // RecoveryPasswordCommand = new AsyncLambdaCmd(OnRecoveryPasswordCommandExecuted,
        //     ex => statusSc.ChangeStatus(ex.Message),
        //     CanRecoveryPasswordCommandExecute);
        
        
        RecoveryPasswordCommand = ReactiveCommand.CreateFromTask(OnRecoveryPasswordCommandExecuted,CanRecoveryPasswordCommandExecute());

        #endregion

        #region Навигационные

        OpenAuthPageCommand = new NavigationCommand(authPageNavigationScs);

        #endregion

        #endregion

        GetQuestionList();
    }

    #region Методы

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

    #endregion

    #region Команды

    public ICommand OpenAuthPageCommand { get; set; }
    public IReactiveCommand RecoveryPasswordCommand { get; }

    

    private IObservable<bool> CanRecoveryPasswordCommandExecute() => 
        this.WhenAnyValue(x => x.Login,x=>x.Password,
            x=>x.QuestionAnswer,
            x=>x.SelectedQuestion,
        (login, password, questionAnswer, selectedQuestion) => 
            !string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password) &&
            !string.IsNullOrEmpty(questionAnswer) && selectedQuestion is not null);
    
    private async Task OnRecoveryPasswordCommandExecuted()
    {
        var base64Sha1Password = await _encryptSc.Sha1Encrypt(Password);

        base64Sha1Password = await _encryptSc.Base64Encrypt(base64Sha1Password);

        var recoveryModel = new RecoveryModel
        {
            QuestionAnswer = QuestionAnswer,
            Question = SelectedQuestion,
            RecoveryAccount = new RegistrationUser { Login = Login, Password = base64Sha1Password }
        };


        if (await _recoveryPasswordSc.RecoveryPassword(recoveryModel)) OpenAuthPageCommand.Execute(null);
    }

    #endregion

    #region Данные с окна

    private bool _canServerConnect;

    public bool CanServerConnect
    {
        get => _canServerConnect;
        set => this.RaiseAndSetIfChanged(ref _canServerConnect, value);
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

    private string? _questionAnswer;

    public string? QuestionAnswer
    {
        get => _questionAnswer;
        set => this.RaiseAndSetIfChanged(ref _questionAnswer, value);
    }


    private ObservableCollection<Question>? _questionsCollection;

    public ObservableCollection<Question>? QuestionsCollection
    {
        get => _questionsCollection;
        set => this.RaiseAndSetIfChanged(ref _questionsCollection, value);
    }


    private Question? _selectedQuestion;

    public Question? SelectedQuestion
    {
        get => _selectedQuestion;
        set => this.RaiseAndSetIfChanged(ref _selectedQuestion, value);
    }

    #endregion
}