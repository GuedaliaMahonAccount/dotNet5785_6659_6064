
namespace DO;
/// <summary>
/// Represents a volunteer assignment with start and end details.
/// </summary>
/// <param name="Id">Assignment ID.</param>
/// <param name="CallId">Associated call ID.</param>
/// <param name="VolunteerId">Volunteer ID.</param>
/// <param name="StartTime">Assignment start time.</param>
/// <param name="EndTime">Optional end time.</param>
/// <param name="EndType">Optional end type.</param>
public record Assignment
(
    int Id,
    int CallId,
    int VolunteerId,
    DateTime StartTime,
    DateTime? EndTime = null,
    Enum? EndType = null
)
{
    public Assignment() : this(0, 0, 0, DateTime.MinValue, null, null) { }
}

