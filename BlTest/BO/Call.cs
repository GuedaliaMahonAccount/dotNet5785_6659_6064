using System;
using System.Collections.Generic;

namespace BO
{
    /// <summary>
    /// Represents a call with details such as type, location, timing, optional description, deadline,
    /// and a list of assignments associated with the call.
    /// </summary>
    public class Call
    {
        /// <summary>
        /// The unique identifier for the call.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// The type of call.
        /// </summary>
        public CallType CallType { get; set; }

        /// <summary>
        /// The address where the call is located.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The latitude coordinate of the call location.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The longitude coordinate of the call location.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// The start time of the call.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// An optional description of the call.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// An optional deadline for completing the call.
        /// </summary>
        public DateTime? DeadLine { get; set; }

        /// <summary>
        /// A list of assignments associated with the call, past and present.
        /// This property is initialized as an empty list to ensure it is never null.
        /// </summary>
        public List<CallAssignInList> Assignments { get; set; } = new List<CallAssignInList>();

        /// <summary>
        /// Overrides ToString for debugging purposes.
        /// </summary>
        /// <returns>A string representation of the Call object.</returns>
        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}
