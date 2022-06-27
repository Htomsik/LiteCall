using LiteCall.Model.Statuses;

namespace LiteCall.Services.Interfaces;

internal interface IStatusServices
{
    public void ChangeStatus(StatusMessage newStatusMessage);
    
    public void ChangeStatus(StatusesActions action);
    
    public void ChangeStatus(StatusesErrors errors);
    
    public void ChangeStatus(string commandError);

    public void DeleteStatus();
}