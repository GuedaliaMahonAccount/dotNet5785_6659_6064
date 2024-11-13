using static DO.CallType;

namespace DO
{
    /// <summary>
    /// Represents a  call with details such as type, location, timing, and optional description and deadline.
    /// </summary>
    /// <param name="Id">The unique identifier for the call.</param>
    /// <param name="CallType">The type of call.</param>
    /// <param name="Address">The address where the call is located.</param>
    /// <param name="Latitude">The latitude coordinate of the call location.</param>
    /// <param name="Longitude">The longitude coordinate of the call location.</param>
    /// <param name="StartTime">The start time of the call.</param>
    /// <param name="Description">An optional description of the call.</param>
    /// <param name="DeadLine">An optional deadline for completing the call.</param>

    public record Call
    (
        int Id,
        CallType CallType,
        string Address,
        double Latitude,
        double Longitude,
        DateTime StartTime,
        string? Description = null,
        DateTime? DeadLine = null
    )
    {
        /// <summary>
        /// Default constructor for a call
        /// </summary>
        public Call() : this(0, CallType.Open, "", 0, 0, DateTime.Now, null, null) { }
    }
}
