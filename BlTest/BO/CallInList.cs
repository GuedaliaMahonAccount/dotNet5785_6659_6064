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
        public int CallId { get; set; }

        /// <summary>
        /// The type of the call.
        /// </summary>
        public CallType CallType { get; set; }

        /// <summary>
        /// The start time of the call.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The total duration of the call.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// The name of the last volunteer assigned to the call.
        /// </summary>
        public string LastVolunteerName { get; set; }

        /// <summary>
        /// The time passed since the last volunteer's completion of their assignment.
        /// </summary>
        public TimeSpan LastCompletionDuration { get; set; }

        /// <summary>
        /// The current status of the call.
        /// </summary>
        public CallType Status { get; set; }

        /// <summary>
        /// The number of assignments associated with the call.
        /// </summary>
        public int AssignmentCount { get; set; }

        /// <summary>
        /// Overrides ToString for debugging purposes.
        /// </summary>
        /// <returns>A string representation of the CallInList object.</returns>
        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}
