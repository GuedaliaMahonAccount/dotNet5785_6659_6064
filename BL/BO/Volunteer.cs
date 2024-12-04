
using System;

namespace BO
{
    /// <summary>
    /// Represents a volunteer with personal information, location details, 
    /// role, activity status, maximum distance for assignments, and preferred distance type.
    /// Includes a call that is currently being handled by the volunteer, if applicable.
    /// </summary>
    public class Volunteer
    {
        /// <summary>
        /// Unique identifier of the volunteer.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Full name of the volunteer.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Phone number of the volunteer.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Email address of the volunteer.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Indicates if the volunteer is currently active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Role assigned to the volunteer.
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Preferred type of distance calculation for the volunteer.
        /// </summary>
        public DistanceType DistanceType { get; set; }

        /// <summary>
        /// Password for the volunteer's account.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Physical address of the volunteer.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Latitude of the volunteer's location.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Longitude of the volunteer's location.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Maximum distance the volunteer is willing to travel.
        /// </summary>
        public double? MaxDistance { get; set; }

        /// <summary>
        /// The call currently being handled by the volunteer, if any.
        /// </summary>
        public CallInProgress? CurrentCall { get; set; }

        /// <summary>
        /// Overrides ToString for debugging purposes.
        /// </summary>
        /// <returns>A string representation of the Volunteer object.</returns>
        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}
