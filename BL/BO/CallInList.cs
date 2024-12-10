using Helpers;

namespace BO
{
    /// <summary>
    /// Represents a summary of a call, including key details for management and reporting.
    /// </summary>
    /// <param name="CallId">The unique identifier for the call.</param>
    /// <param name="CallType">The type of the call.</param>
    /// <param name="StartTime">The start time of the call.</param>
    /// <param name="Duration">The total duration of the call.</param>
    /// <param name="LastVolunteerName">The name of the last volunteer assigned to the call.</param>
    /// <param name="LastCompletionDuration">The time passed since the last volunteer's completion of their assignment.</param>
    /// <param name="Status">The current status of the call.</param>
    /// <param name="AssignmentCount">The number of assignments associated with the call.</param>
    public class CallInList
    {
        public int? CallId { get; init; }
        public CallType CallType { get; init; }
        public DateTime StartTime { get; init; }
        public TimeSpan? LeftTimeToExpire { get; init; }
        public string? LastVolunteerName { get; init; }
        public TimeSpan? LeftTimeTocomplete { get; init; }
        public CallType Status { get; init; }
        public int AssignmentCount { get; init; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
