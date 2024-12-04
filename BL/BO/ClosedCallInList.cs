using Helpers;
using System;

namespace BO
{
    /// <summary>
    /// Represents a closed call in a list, including details such as type, timing, and resolution.
    /// This is a read-only entity for viewing purposes only.
    /// </summary>
    public class ClosedCallInList
    {
        /// <summary>
        /// The unique identifier for the call.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// The type of the call.
        /// </summary>
        public CallType CallType { get; init; }

        /// <summary>
        /// A description of the call.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// The time the call started.
        /// </summary>
        public DateTime StartTime { get; init; }

        /// <summary>
        /// The time the call was closed.
        /// </summary>
        public DateTime EndTime { get; init; }

        /// <summary>
        /// The time the resolution was completed.
        /// </summary>
        public DateTime? ResolutionTime { get; init; }

        /// <summary>
        /// The type of resolution for the call.
        /// </summary>
        public EndType EndType { get; init; }

        /// <summary>
        /// Overrides ToString for debugging purposes.
        /// </summary>
        /// <returns>A string representation of the ClosedCallInList object.</returns>
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
