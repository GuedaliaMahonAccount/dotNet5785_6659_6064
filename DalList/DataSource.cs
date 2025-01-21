using DO;

namespace Dal;

internal static class DataSource
{
    [MethodImpl(MethodImplOptions.Synchronized)]

    internal static List<DO.Assignment?> Assignments { get; } = new();
    internal static List<DO.Call?> Calls { get; } = new();
    internal static List<DO.Volunteer?> Volunteers { get; } = new();
}
