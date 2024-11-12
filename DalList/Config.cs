namespace Dal
{
    /// <summary>
    /// Config class to manage unique ID generation, system clock, and risk range settings.
    /// Provides a method to reset all configurations to their initial states.
    /// </summary>
    internal static class Config
    {
        // Starting ID for call-related operations
        const int startCallId = 1;
        static int nextCallId = startCallId;

        /// <summary>
        /// Gets the next unique call ID.
        /// </summary>
        internal static int NextCallId
        {
            get => nextCallId++;
        }

        // Starting ID for assignment-related operations
        const int startAssignmentId = 1;
        static int nextAssignmentId = startAssignmentId;

        /// <summary>
        /// Gets the next unique assignment ID.
        /// </summary>
        internal static int NextAssignmentId
        {
            get => nextAssignmentId++;
        }

        /// <summary>
        /// System clock representing the current time in the application.
        /// </summary>
        internal static DateTime Clock { get; set; } = DateTime.Now;

        /// <summary>
        /// Defines the time range for risk assessment operations. 1 hour before the dead line, it become risky
        /// </summary>
        internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(1);

        /// <summary>
        /// Resets all configuration variables to their initial values.
        /// Resets the call and assignment IDs, sets the clock to the current time,
        /// and initializes the risk range to a default of 1 hours.
        /// </summary>
        internal static void Reset()
        {
            nextCallId = startCallId;
            nextAssignmentId = startAssignmentId;
            Clock = DateTime.Now;
            RiskRange = TimeSpan.FromHours(1);
        }
    }
}
