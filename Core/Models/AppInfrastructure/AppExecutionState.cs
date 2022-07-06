using Core.Models.AppInfrastructure.StateStatuses;

namespace Core.Models.AppInfrastructure;

public class AppExecutionState
{
    public string? Message { get; set; }

    public StateTypes Type { get; set; }
}
