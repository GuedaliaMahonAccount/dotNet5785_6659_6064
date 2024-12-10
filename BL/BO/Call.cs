using Helpers;

namespace BO
{
    /// <summary>
    /// Represents a call with details such as type, location, timing, optional description, deadline,
    /// and a list of assignments associated with the call.
    /// </summary>
    /// <param name="Id">The unique identifier for the call.</param>
    /// <param name="CallType">The type of call.</param>
    /// <param name="Address">The address where the call is located.</param>
    /// <param name="Latitude">The latitude coordinate of the call location.</param>
    /// <param name="Longitude">The longitude coordinate of the call location.</param>
    /// <param name="StartTime">The start time of the call.</param>
    /// <param name="Description">An optional description of the call.</param>
    /// <param name="DeadLine">An optional deadline for completing the call.</param>
    /// <param name="Assignments">A list of assignments associated with the call, past and present.</param>
    public class Call
    {
        public int Id { get; init; }
        public CallType CallType { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime StartTime { get; set; }
        public string? Description { get; set; }
        public DateTime? DeadLine { get; set; }
        public List<CallAssignInList>? Assignments { get; set; } = new List<CallAssignInList>();

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
