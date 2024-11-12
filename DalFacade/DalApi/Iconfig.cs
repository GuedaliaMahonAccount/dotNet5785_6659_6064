
namespace DalApi;

/// <summary>
/// Config class to manage unique ID generation, system clock, and risk range settings.
/// </summary>
public interface IConfig
{
    int NextCallId { get; }
    int NextAssignmentId { get; }
    DateTime Clock { get; set; }
    TimeSpan RiskRange { get; set; }

}
