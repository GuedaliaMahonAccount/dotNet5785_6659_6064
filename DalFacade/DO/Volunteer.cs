using static DO.Role;
using static DO.DistanceType;
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
    Boolean IsActive,
    Role Role,
    DistanceType DistanceType,
    string? Password=null,
    string? Address = null,
    double? Latitude = null,
    double? Longitude = null,
    double? MaxDistance = null
)
{
    /// <summary>
    /// Default constructor for vonlonter
    /// </summary>
    public Volunteer() : this(0, "", "", "",false, Role.Volunteer, DistanceType.Foot, null, null, null, null, null) { }
   
}
