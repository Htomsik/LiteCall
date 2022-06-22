using LiteCall.Model;

namespace LiteCall.Services.Interfaces;

internal interface IStatusServices
{
    public void ChangeStatus(StatusMessage newStatusMessage);

    public void DeleteStatus();
}