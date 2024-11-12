namespace Dal
{
    internal static class Config
    {
        const int startCallId = 1;
        static int nextCallId = startCallId;
        internal static int NextCallId { get => nextCallId++; }
        const int startAssignmentId = 100;
        static int nextAssignmentId = startAssignmentId;
        internal static int NextAssignmentId { get => nextAssignmentId++; }
        internal static DateTime Clock { get; set; } = DateTime.Now;
        internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(4);
        internal static void Reset()
        {
            nextCallId = startCallId;
            nextAssignmentId = startAssignmentId;
            Clock = DateTime.Now;
            RiskRange = TimeSpan.FromHours(4);
        }
    }
}
