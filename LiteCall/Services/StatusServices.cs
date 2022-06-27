using System.Collections.Generic;
using System.Threading.Tasks;
using LiteCall.Model.Statuses;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services;

internal sealed class StatusServices : IStatusServices
{
    private readonly StatusMessageStore _statusMessageStore;
    private bool _isDelete;

    public StatusServices(StatusMessageStore statusMessageStore)
    {
        _statusMessageStore = statusMessageStore;
    }

    public async void ChangeStatus(StatusMessage newStatusMessage)
    {
        if (!_isDelete)
            _statusMessageStore.CurrentStatusMessage = newStatusMessage;
        else
            return;
        if (newStatusMessage.Type != StatusType.Error) return;
        
        _isDelete = true;

        await TimerDelete(4000);
    }


    public async void ChangeStatus(StatusesActions action)
    {
        ChangeStatus(SampleActions[action]);
    }
    
    public async void ChangeStatus(StatusesErrors error)
    {
        ChangeStatus(SampleErrors[error]);
    }

    public async void ChangeStatus(string message)
    {
        ChangeStatus(new StatusMessage{Message = message, Type = StatusType.Error});
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

    private static readonly Dictionary<StatusesActions, StatusMessage> SampleActions =
        new Dictionary<StatusesActions, StatusMessage>
        {
            {StatusesActions.GettingCaptcha, new StatusMessage{Message = "Getting a captcha from the server. . .", Type = StatusType.Action}},
            {StatusesActions.ServerConnection, new StatusMessage{Message = "Connection to the server. . .",Type = StatusType.Action}},
            {StatusesActions.CheckingServerStatus, new StatusMessage{Message = "Checking the server status. . .",Type = StatusType.Action}},
            { StatusesActions.GettingInfoAboutServer, new StatusMessage{Message = "Getting information about the server. . .",Type = StatusType.Action} }
        };

    private static readonly Dictionary<StatusesErrors, StatusMessage> SampleErrors =
        new Dictionary<StatusesErrors, StatusMessage>
        {
            {StatusesErrors.ServerConnectionFailed, new StatusMessage{Message = "The connection to the server failed", Type = StatusType.Error}},
            {StatusesErrors.IncorrectServerNameOrIp, new StatusMessage{Message = "Incorrect server name or Ip", Type = StatusType.Error}},
            {StatusesErrors.IncorrectServerIp, new StatusMessage{Message = "Incorrect server Ip", Type = StatusType.Error}},
            { StatusesErrors.UnknownError, new StatusMessage{Message = "Unknown Error", Type = StatusType.Error}},
            {StatusesErrors.AuthorizationFailed,new StatusMessage{Message = "Authorization failed. Check username or password.", Type = StatusType.Error}},
            {StatusesErrors.RegistrationFailed, new StatusMessage{Message = "Registration failed. Maybe this username is already exist",Type = StatusType.Error}}
        };

}


