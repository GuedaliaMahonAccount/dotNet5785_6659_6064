/// <summary>
/// Represents an open call in a list, including details such as type, timing, and distance to the caller.
/// This is a read-only entity for viewing purposes only.
/// </summary>
/// <param name="Id">The unique identifier for the call.</param>
/// <param name="CallType">The type of the call.</param>
/// <param name="Description">A description of the call.</param>
/// <param name="CallerName">The name of the caller.</param>
/// <param name="StartTime">The time the call started.</param>
/// <param name="LastUpdateTime">The time the call was last updated.</param>
/// <param name="Distance">The distance between the call's location and the responder.</param>
using Helpers;
using System;

namespace BO
{
    public class OpenCallInList
    {
        public int Id { get; init; }
        public CallType CallType { get; init; }
        public string? Description { get; init; }
        public string Address { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime? MaxEndTime { get; init; }
        public double Distance { get; init; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
