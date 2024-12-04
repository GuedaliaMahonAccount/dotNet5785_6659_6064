using Helpers;
using System;

namespace BO
{
    /// <summary>
    /// Represents a summary of a call, including key details for management and reporting.
    /// </summary>
    public class CallInList
    {
        /// <summary>
        /// The unique identifier for the call.
        /// </summary>
        public int CallId { get; init; }

        /// <summary>
        /// The type of the call.
        /// </summary>
        public CallType CallType { get; init; }

        /// <summary>
        /// The start time of the call.
        /// </summary>
        public DateTime StartTime { get; init; }

        /// <summary>
        /// The total duration of the call.
        /// </summary>
        public TimeSpan Duration { get; init; }

        /// <summary>
        /// The name of the last volunteer assigned to the call.
        /// </summary>
        public string LastVolunteerName { get; init; }

        /// <summary>
        /// The time passed since the last volunteer's completion of their assignment.
        /// </summary>
        public TimeSpan LastCompletionDuration { get; init; }

        /// <summary>
        /// The current status of the call.
        /// </summary>
        public CallType Status { get; init; }

        /// <summary>
        /// The number of assignments associated with the call.
        /// </summary>
        public int AssignmentCount { get; init; }

        /// <summary>
        /// Overrides ToString for debugging purposes.
        /// </summary>
        /// <returns>A string representation of the CallInList object.</returns>
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
