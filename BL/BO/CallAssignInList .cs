using System;

namespace BO
{
    /// <summary>
    /// Represents an assignment within a call list, including details about the volunteer, timing, and type of assignment.
    /// </summary>
    public class CallAssignInList
    {
        /// <summary>
        /// The unique identifier of the volunteer assigned to the call.
        /// </summary>
        public int? VolunteerId { get; init; }

        /// <summary>
        /// The name of the assigned volunteer.
        /// </summary>
        public string VolunteerName { get; init; }

        /// <summary>
        /// The time the volunteer started the assignment.
        /// </summary>
        public DateTime? StartTime { get; init; }

        /// <summary>
        /// The time the volunteer completed the assignment.
        /// </summary>
        public DateTime? EndTime { get; init; }

        /// <summary>
        /// The type of assignment.
        /// </summary>
        public EndType EndType { get; init; }

        /// <summary>
        /// Overrides ToString for debugging purposes.
        /// </summary>
        /// <returns>A string representation of the CallAssignInList object.</returns>
        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}
