/// <summary>
/// Represents an assignment within a call list, including details about the volunteer, timing, and type of assignment.
/// </summary>
/// <param name="VolunteerId">The unique identifier of the volunteer assigned to the call.</param>
/// <param name="VolunteerName">The name of the assigned volunteer.</param>
/// <param name="StartTime">The time the volunteer started the assignment.</param>
/// <param name="EndTime">The time the volunteer completed the assignment.</param>
/// <param name="EndType">The type of assignment.</param>
using Helpers;
using System;

namespace BO
{
    public class CallAssignInList
    {
        public int? VolunteerId { get; init; }
        public string VolunteerName { get; init; }
        public DateTime? StartTime { get; init; }
        public DateTime? EndTime { get; init; }
        public EndType EndType { get; init; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
