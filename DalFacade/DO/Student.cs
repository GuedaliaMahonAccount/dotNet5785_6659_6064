// Module Student.cs
namespace DO;

/// <summary>
/// Student Entity represents a student with all its props
/// </summary>
/// <param name="Id">Personal unique ID of the student (as in national id card)</param>
/// <param name="Name">Private Name of the student</param>
/// <param name="RegistrationDate">Registration date of the student into the graduation program</param>
/// <param name="Alias">student’s alias name (default empty)</param>
/// <param name="IsActive">whether the student is active in studies (default true)</param>
/// <param name="BirthDate">student’s birthday (default empty)</param>
public record Student
(
    int Id,
    string Name,
    DateTime RegistrationDate,
    string? Alias = null,
    bool IsActive = false,
    DateTime? BirthDate = null
)
{
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Student() : this(0) { }
}

