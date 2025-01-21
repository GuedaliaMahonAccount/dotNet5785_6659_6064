namespace Dal
{
    using DalApi;

    /// <summary>
    /// Implementation of IConfig interface, delegating properties and methods to the Config class.
    /// </summary>
    internal class ConfigImplementation : IConfig
    {


        // Gets the next unique call ID.
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int NextCallId => Config.NextCallId;

        // Gets the next unique assignment ID.
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int NextAssignmentId => Config.NextAssignmentId;

        // Gets or sets the system clock representing the current time in the application.
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DateTime Clock
        {
            get => Config.Clock;
            set => Config.Clock = value;
        }

        // Gets or sets the time range for risk assessment operations.
        [MethodImpl(MethodImplOptions.Synchronized)]
        public TimeSpan RiskRange
        {
            get => Config.RiskRange;
            set => Config.RiskRange = value;
        }

        // Resets all configuration variables to their initial values.
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Reset()
        {
            Config.Reset();
        }
    }
}
