using Helpers;
using System;

namespace BO
{
    /// <summary>
    /// Represents an open call in a list, including details such as type, timing, and distance to the caller.
    /// This is a read-only entity for viewing purposes only.
    /// </summary>
    public class OpenCallInList
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
        /// The name of the caller.
        /// </summary>
        public string CallerName { get; init; }

        /// <summary>
        /// The time the call started.
        /// </summary>
        public DateTime StartTime { get; init; }

        /// <summary>
        /// The time the call was last updated.
        /// </summary>
        public DateTime LastUpdateTime { get; init; }

        /// <summary>
        /// The distance between the call's location and the responder.
        /// </summary>
        public double Distance { get; init; }

        /// <summary>
        /// Overrides ToString for debugging purposes.
        /// </summary>
        /// <returns>A string representation of the OpenCallInList object.</returns>
        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
