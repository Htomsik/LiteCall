using LiteCall.Model;
using LiteCall.Model.Errors;

namespace LiteCall.Services.Interfaces;

internal interface IStatusServices
{
    public void ChangeStatus(StatusMessage newStatusMessage);

    public void DeleteStatus();

}