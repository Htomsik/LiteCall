using LiteCall.Infrastructure.Commands;
using LiteCall.Infrastructure.Commands.Lambda;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace LiteCall.ViewModels.Pages
{
	internal class RegistrationPageVmd : BaseVmd
	{
		public RegistrationPageVmd(INavigationService authPageNavigationServices,
			IRegistrationServices registrationServices, IStatusServices statusServices,
			ICaptchaServices captchaServices, IGetPassRecoveryQuestionsServices getPassRecoveryQuestionsServices,
			IEncryptServices encryptServices)
		{
			_registrationServices = registrationServices;


			_captchaServices = captchaServices;

			_getPassRecoveryQuestionsServices = getPassRecoveryQuestionsServices;

			_encryptServices = encryptServices;


			RegistrationCommand = new AsyncLambdaCommand(OnRegistrationExecuted,
				(ex) => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),
				CanRegistrationExecute);

			OpenAuthPageCommand = new NavigationCommand(authPageNavigationServices);

			OpenModalCommand = new AsyncLambdaCommand(OnOpenModalCommandExecuted,
				(ex) => statusServices.ChangeStatus(new StatusMessage { IsError = true, Message = ex.Message }),
				CanOpenModalCommandExecute);


			GetQuestionList();
		}


		private async void GetQuestionList()
		{
			try
			{
				QuestionsCollection =
					new ObservableCollection<Question>((await _getPassRecoveryQuestionsServices.GetQuestions())!);

				CanServerConnect = true;
			}
			catch
			{
				CanServerConnect = false;

				OpenAuthPageCommand.Execute(null);
			}
		}

		#region Commands

		public ICommand RegistrationCommand { get; }
		private bool CanRegistrationExecute(object p) => !(bool)p && !string.IsNullOrEmpty(CaptchaString);

		private async Task OnRegistrationExecuted(object p)
		{
			var base64Sha1Password = await _encryptServices.Sha1Encrypt(Password);

			base64Sha1Password = await _encryptServices.Base64Encrypt(base64Sha1Password);

			var newAccount = new Account()
			{
				Login = this.Login,
				Password = base64Sha1Password
			};

			var registrationModel = new RegistrationModel()
			{
				Captcha = CaptchaString,
				QestionAnswer = QuestionAnswer,
				Question = SelectedQuestion,
				RecoveryAccount = newAccount
			};

			var response = await _registrationServices.Registration(registrationModel);

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

				if (!status)
				{
					return;
				}

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
			CaptсhaImageSource = await _captchaServices.GetCaptcha();

			if (CaptсhaImageSource == null)
			{
				return false;
			}

			return true;
		}


		public ICommand OpenAuthPageCommand { get; }

		#endregion


		private bool _canServerConnect;

		public bool CanServerConnect
		{
			get => _canServerConnect;
			set => Set(ref _canServerConnect, value);
		}


		private bool _modalStatus;

		public bool ModalStatus
		{
			get => _modalStatus;
			set => Set(ref _modalStatus, value);
		}


		private string? _captchaString;

		public string? CaptchaString
		{
			get => _captchaString;
			set => Set(ref _captchaString, value);
		}


		private ImageSource? _captсhaImageSource;

		public ImageSource? CaptсhaImageSource
		{
			get => _captсhaImageSource;
			set => Set(ref _captсhaImageSource, value);
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


		private string? _confirmPassword;

		public string? ConfirmPassword
		{
			get => _confirmPassword;
			set => Set(ref _confirmPassword, value);
		}


		private string? _questionAnswer;

		public string? QuestionAnswer
		{
			get => _questionAnswer;
			set => Set(ref _questionAnswer, value);
		}


		private ObservableCollection<Question>? _questionsCollection;

		public ObservableCollection<Question>? QuestionsCollection
		{
			get => _questionsCollection;
			set => Set(ref _questionsCollection, value);
		}


		private Question? _selectedQuestion;

		public Question? SelectedQuestion
		{
			get => _selectedQuestion;
			set => Set(ref _selectedQuestion, value);
		}


		private readonly IRegistrationServices _registrationServices;

		private readonly ICaptchaServices _captchaServices;

		private readonly IGetPassRecoveryQuestionsServices _getPassRecoveryQuestionsServices;

		private readonly IEncryptServices _encryptServices;
	}
}