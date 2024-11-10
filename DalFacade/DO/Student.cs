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

// Module Course.cs
namespace DO;

/// <summary>
/// Course Entity
/// </summary>
/// <param name="Id">unique ID (created automatically - provide 0 as an argument)</param>
/// <param name="CourseNumber">Course official number</param>
/// <param name="Name">Course official name</param>
/// <param name="InSemester">Course’s semester</param>
/// <param name="InYear">Course’s year</param>
/// <param name="DayInWeek">Which weekday the course is given</param>
/// <param name="StartTime">Course lesson start time</param>
/// <param name="EndTime">Course lesson end time</param>
/// <param name="Credits">Amount of credits given for successful completing the course</param>
public record Course
(
    int Id,
    string CourseNumber,
    string CourseName,
    Year? InYear = null,
    SemesterNames? InSemester = null,
    WeekDay? DayInWeek = null,
    TimeSpan? StartTime = null,
    TimeSpan? EndTime = null,
    int? Credits = null
)
{
    /// <summary>
    /// Default constructor for stage 3
    /// </summary>
    public Course() : this(0, "", "") { }
}
