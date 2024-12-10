using Helpers;

namespace BO
{
    /// <summary>
    /// Represents a volunteer in a list, including personal information, activity status, and assignment counts.
    /// This is a read-only entity for viewing purposes.
    /// </summary>
    /// <param name="Id">Unique identifier of the volunteer.</param>
    /// <param name="Name">Full name of the volunteer.</param>
    /// <param name="IsActive">Indicates if the volunteer is currently active.</param>
    /// <param name="CompletedAssignmentsCount">Number of assignments completed by the volunteer.</param>
    /// <param name="InProgressAssignmentsCount">Number of assignments currently in progress for the volunteer.</param>
    /// <param name="CurrentCallId">Unique identifier of the current call assigned to the volunteer, if any.</param>
    /// <param name="CurrentCallType">Type of the current call assigned to the volunteer, if any.</param>
    public class VolunteerInList
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public bool IsActive { get; init; }
        public int CompletedAssignmentsCount { get; init; }
        public int InProgressAssignmentsCount { get; init; }
        public int? CurrentCallId { get; init; }
        public CallType CurrentCallType { get; init; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
