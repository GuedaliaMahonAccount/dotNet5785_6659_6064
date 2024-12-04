using Helpers;
using System;

namespace BO
{
    /// <summary>
    /// Represents a call currently being handled by a volunteer.
    /// </summary>
    public class CallInProgress
    {
        /// <summary>
        /// Unique identifier of the assignment (volunteer to call relationship).
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Unique identifier of the call.
        /// </summary>
        public int CallId { get; init; }

        /// <summary>
        /// Type of the call.
        /// </summary>
        public CallType CallType { get; init; }

        /// <summary>
        /// A general description of the call.
        /// </summary>
        public string GeneralDescription { get; init; }

        /// <summary>
        /// Additional notes related to the call.
        /// </summary>
        public string AdditionalNotes { get; init; }

        /// <summary>
        /// The time the call started.
        /// </summary>
        public DateTime StartTime { get; init; }

        /// <summary>
        /// The estimated completion time for the call.
        /// </summary>
        public DateTime? EstimatedCompletionTime { get; init; }

        /// <summary>
        /// The time the assignment was made to the volunteer.
        /// </summary>
        public DateTime AssignmentStartTime { get; init; }

        /// <summary>
        /// The distance between the volunteer and the call's location.
        /// </summary>
        public double Distance { get; init; }

        /// <summary>
        /// The current status of the call in progress.
        /// </summary>
        public CallType Status { get; init; }

        /// <summary>
        /// Overrides ToString for debugging purposes.
        /// </summary>
        /// <returns>A string representation of the CallInProgress object.</returns>
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
