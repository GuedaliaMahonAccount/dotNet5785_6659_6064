namespace DO;

/// <summary>
/// Represents a volunteer with personal information, location details, 
/// role, activity status, maximum distance for assignments, and preferred distance type.
/// </summary>
/// <param name="Id">Unique identifier of the volunteer.</param>
/// <param name="Name">Full name of the volunteer.</param>
/// <param name="Phone">Phone number of the volunteer.</param>
/// <param name="Email">Email address of the volunteer.</param>
/// <param name="Password">Password for the volunteer's account.</param>
/// <param name="Address">Physical address of the volunteer.</param>
/// <param name="Latitude">Latitude of the volunteer's location.</param>
/// <param name="Longitude">Longitude of the volunteer's location.</param>
/// <param name="Role">Role assigned to the volunteer.</param>
/// <param name="IsActive">Indicates if the volunteer is currently active.</param>
/// <param name="MaxDistance">Maximum distance the volunteer is willing to travel.</param>
/// <param name="DistanceType">Type of distance calculation preferred by the volunteer.</param>
public record Volunteer
(
    int Id,
    string Name,
    string Phone,
    string Email,
    string? Password=null,
    string? Address = null,
    double? Latitude = null,
    double? Longitude = null,
    Role Role = Role.Volunteer,
    Boolean IsActive = false,
    double? MaxDistance = null,
    DistanceType DistanceType= DistanceType.Plane
)
{
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Volunteer() : this(0, "", "", "", null, null, null, null, Role.Volunteer, true, null, DistanceType.Plane) { }

}
