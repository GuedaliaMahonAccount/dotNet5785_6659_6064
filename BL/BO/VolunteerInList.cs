using System;

namespace BO
{
    /// <summary>
    /// Represents a volunteer in a list, including personal information, activity status, and assignment counts.
    /// This is a read-only entity for viewing purposes.
    /// </summary>
    public class VolunteerInList
    {
        /// <summary>
        /// Unique identifier of the volunteer.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Full name of the volunteer.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Indicates if the volunteer is currently active.
        /// </summary>
        public bool IsActive { get; init; }

        /// <summary>
        /// Number of assignments completed by the volunteer.
        /// </summary>
        public int CompletedAssignmentsCount { get; init; }

        /// <summary>
        /// Number of assignments currently in progress for the volunteer.
        /// </summary>
        public int InProgressAssignmentsCount { get; init; }

        /// <summary>
        /// Unique identifier of the current call assigned to the volunteer, if any.
        /// </summary>
        public int? CurrentCallId { get; init; }

        /// <summary>
        /// Type of the current call assigned to the volunteer, if any.
        /// </summary>
        public CallType CurrentCallType { get; init; }

        /// <summary>
        /// Overrides ToString for debugging purposes.
        /// </summary>
        /// <returns>A string representation of the VolunteerInList object.</returns>
        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}
