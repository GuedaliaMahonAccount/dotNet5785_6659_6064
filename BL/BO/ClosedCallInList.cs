using Helpers;

namespace BO
{
    /// <summary>
    /// Represents a closed call in a list, including details such as type, timing, and resolution.
    /// This is a read-only entity for viewing purposes only.
    /// </summary>
    /// <param name="Id">The unique identifier for the call.</param>
    /// <param name="CallType">The type of the call.</param>
    /// <param name="Description">A description of the call.</param>
    /// <param name="StartTime">The time the call started.</param>
    /// <param name="EndTime">The time the call was closed.</param>
    /// <param name="ResolutionTime">The time the resolution was completed.</param>
    /// <param name="EndType">The type of resolution for the call.</param>
    public class ClosedCallInList
    {
        public int Id { get; init; }
        public CallType CallType { get; init; }
        public string Address { get; init; }
        public DateTime OpenTime { get; init; }
        public DateTime StartAssignementTime { get; init; }
        public DateTime? EndTime { get; init; }
        public EndType? EndType { get; init; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
