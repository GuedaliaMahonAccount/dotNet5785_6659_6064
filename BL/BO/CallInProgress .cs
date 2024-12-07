/// <summary>
/// Represents a call currently being handled by a volunteer.
/// </summary>
/// <param name="Id">Unique identifier of the assignment (volunteer to call relationship).</param>
/// <param name="CallId">Unique identifier of the call.</param>
/// <param name="CallType">Type of the call.</param>
/// <param name="GeneralDescription">A general description of the call.</param>
/// <param name="AdditionalNotes">Additional notes related to the call.</param>
/// <param name="StartTime">The time the call started.</param>
/// <param name="EstimatedCompletionTime">The estimated completion time for the call.</param>
/// <param name="AssignmentStartTime">The time the assignment was made to the volunteer.</param>
/// <param name="Distance">The distance between the volunteer and the call's location.</param>
/// <param name="Status">The current status of the call in progress.</param>
using Helpers;
using System;

namespace BO
{
    public class CallInProgress
    {
        public int Id { get; init; }
        public int CallId { get; init; }
        public CallType CallType { get; init; }
        public string? GeneralDescription { get; init; }
        public string Address { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime? EstimatedCompletionTime { get; init; }
        public DateTime AssignmentStartTime { get; init; }
        public double Distance { get; init; }
        public CallType Status { get; init; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
