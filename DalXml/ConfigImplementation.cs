namespace Dal
{
    using DalApi;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Implementation of IConfig interface, delegating properties and methods to the Config class.
    /// </summary>
    internal class ConfigImplementation : IConfig
    {

        // Gets the next unique call ID.
        public int NextCallId
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return Config.NextCallId;
            }
        }

        // Gets the next unique assignment ID.
        public int NextAssignmentId
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return Config.NextAssignmentId;
            }
        }

        // Gets or sets the system clock representing the current time in the application.
        public DateTime Clock
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => Config.Clock;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set => Config.Clock = value;
        }

        // Gets or sets the time range for risk assessment operations.
        public TimeSpan RiskRange
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => Config.RiskRange;
            [MethodImpl(MethodImplOptions.Synchronized)]
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
