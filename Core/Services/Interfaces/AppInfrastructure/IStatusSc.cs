using Core.Models.AppInfrastructure;
using Core.Models.AppInfrastructure.StateStatuses;

namespace Core.Services.Interfaces.AppInfrastructure;

public interface IStatusSc
{
    public void ChangeStatus(AppExecutionState newStatusMessage);

    public void ChangeStatus(ExecutionActionStates action);

    public void ChangeStatus(ExecutionErrorStates errors);

    public void ChangeStatus(string commandError);

    public void DeleteStatus();
}