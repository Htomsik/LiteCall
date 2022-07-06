using Core.Models.AppInfrastructure;
using Core.Models.AppInfrastructure.StateStatuses;
using Core.Services.Interfaces.AppInfrastructure;
using Core.Stores.AppInfrastructure;

namespace Core.Services.AppInfrastructure;

public sealed class AppExecutionStateS : IStatusSc
{
    private static readonly Dictionary<ExecutionActionStates, AppExecutionState> SampleActions =
        new()
        {
            {
                ExecutionActionStates.GettingCaptcha,
                new AppExecutionState { Message = "Getting a captcha from the server. . .", Type = StateTypes.Action }
            },
            {
                ExecutionActionStates.ServerConnection,
                new AppExecutionState { Message = "Connection to the server. . .", Type = StateTypes.Action }
            },
            {
                ExecutionActionStates.CheckingServerStatus,
                new AppExecutionState { Message = "Checking the server status. . .", Type = StateTypes.Action }
            },
            {
                ExecutionActionStates.GettingInfoAboutServer,
                new AppExecutionState { Message = "Getting information about the server. . .", Type = StateTypes.Action }
            }
        };

    private static readonly Dictionary<ExecutionErrorStates, AppExecutionState> SampleErrors =
        new()
        {
            {
                ExecutionErrorStates.ServerConnectionFailed,
                new AppExecutionState { Message = "The connection to the server failed", Type = StateTypes.Error }
            },
            {
                ExecutionErrorStates.IncorrectServerNameOrIp,
                new AppExecutionState { Message = "Incorrect server name or Ip", Type = StateTypes.Error }
            },
            {
                ExecutionErrorStates.IncorrectServerIp,
                new AppExecutionState { Message = "Incorrect server Ip", Type = StateTypes.Error }
            },
            { ExecutionErrorStates.UnknownError, new AppExecutionState { Message = "Unknown Error", Type = StateTypes.Error } },
            {
                ExecutionErrorStates.AuthorizationFailed,
                new AppExecutionState
                    { Message = "Authorization failed. Check username or password.", Type = StateTypes.Error }
            },
            {
                ExecutionErrorStates.RegistrationFailed,
                new AppExecutionState
                    { Message = "Registration failed. Maybe this username is already exist", Type = StateTypes.Error }
            }
        };

    private readonly AppExecutionStateStore _statusMessageStore;
    private bool _isDelete;

    public AppExecutionStateS(AppExecutionStateStore statusMessageStore)
    {
        _statusMessageStore = statusMessageStore;
    }

    public async void ChangeStatus(AppExecutionState newStatusMessage)
    {
        if (!_isDelete)
            _statusMessageStore.CurrentStatusMessage = newStatusMessage;
        else
            return;
        if (newStatusMessage.Type != StateTypes.Error) return;

        _isDelete = true;

        await TimerDelete(4000);
    }


    public async void ChangeStatus(ExecutionActionStates action)
    {
        ChangeStatus(SampleActions[action]);
    }

    public async void ChangeStatus(ExecutionErrorStates error)
    {
        ChangeStatus(SampleErrors[error]);
    }

    public async void ChangeStatus(string message)
    {
        ChangeStatus(new AppExecutionState { Message = message, Type = StateTypes.Error });
    }

    public void DeleteStatus()
    {
        if (_isDelete) return;

        _statusMessageStore.CurrentStatusMessage = null;
    }

    private async Task TimerDelete(int delay)
    {
        await Task.Delay(delay);

        _isDelete = false;

        DeleteStatus();
    }
}