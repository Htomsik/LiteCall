using System.Threading.Tasks;
using LiteCall.Model;
using LiteCall.Services.Interfaces;
using LiteCall.Stores;

namespace LiteCall.Services;

internal class StatusServices : IStatusServices
{
    private static bool _isDelete;
    private readonly StatusMessageStore _statusMessageStore;

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
        if (newStatusMessage.IsError)
        {
            _isDelete = true;

            await TimerDelete(4000);
        }
    }

    public void DeleteStatus()
    {
        if (_isDelete) return;

        _statusMessageStore.CurrentStatusMessage = null;
    }

    private async Task TimerDelete(int Delay)
    {
        await Task.Delay(Delay);

        _isDelete = false;

        DeleteStatus();
    }
}