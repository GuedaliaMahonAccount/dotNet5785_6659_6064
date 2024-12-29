using Helpers;

namespace BO
{
    /// <summary>
    /// Represents a volunteer with personal information, location details, 
    /// role, activity status, maximum distance for assignments, and preferred distance type.
    /// Includes a call that is currently being handled by the volunteer, if applicable.
    /// </summary>
    /// <param name="Id">Unique identifier of the volunteer.</param>
    /// <param name="Name">Full name of the volunteer.</param>
    /// <param name="Phone">Phone number of the volunteer.</param>
    /// <param name="Email">Email address of the volunteer.</param>
    /// <param name="IsActive">Indicates if the volunteer is currently active.</param>
    /// <param name="Role">Role assigned to the volunteer.</param>
    /// <param name="DistanceType">Preferred type of distance calculation for the volunteer.</param>
    /// <param name="Password">Password for the volunteer's account.</param>
    /// <param name="Address">Physical address of the volunteer.</param>
    /// <param name="Latitude">Latitude of the volunteer's location.</param>
    /// <param name="Longitude">Longitude of the volunteer's location.</param>
    /// <param name="MaxDistance">Maximum distance the volunteer is willing to travel.</param>
    /// <param name="CurrentCall">The call currently being handled by the volunteer, if any.</param>
    public class Volunteer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public bool IsActive { get; set; }
        public Role Role { get; set; }
        public DistanceType DistanceType { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? MaxDistance { get; set; }
        public CallInProgress? CurrentCall { get; set; }

        public override string ToString()
        {
            return this.ToStringProperty();
        }
    }
}
