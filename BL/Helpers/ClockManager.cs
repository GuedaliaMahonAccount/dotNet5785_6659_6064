using BO;
namespace Helpers;

/// <summary>
/// Internal BL manager for all Application's Clock logic policies
/// </summary>
internal static class ClockManager //stage 4
{
    #region Stage 4
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get; //stage 4

    /// <summary>
    /// Property for providing current application's clock value for any BL class that may need it
    /// </summary>
    internal static DateTime Now
    {
        get
        {
            // No try-catch, throw the exception directly if it occurs
            return _dal.Config.Clock;
        }
    }
    /// <summary>
    /// Method to perform application's clock from any BL class as may be required
    /// </summary>
    /// <param name="newClock">updated clock value</param>
    internal static void UpdateClock(DateTime newClock)
    {
        // No try-catch, throw the exception directly if it occurs
        updateClock(newClock);
    }
    private static void updateClock(DateTime newClock)
    {
        try
        {
            var oldClock = _dal.Config.Clock; // This line could throw an exception, for example, if _dal is null or invalid.
            _dal.Config.Clock = newClock; // If this fails, an exception will be thrown, and we won't catch it here.

            // Add calls to any logic method that should be called periodically, after each clock update.
            // For example: Periodic student updates:
            CallManager.UpdateExpiredCalls(); // This could throw an exception too, and it will be thrown up to the caller.

            // Calling all the observers of the clock update.
            ClockUpdatedObservers?.Invoke(); // This is also susceptible to throwing exceptions.
        }
        catch (Exception ex)
        {
            // Throw the exception upwards without catching locally, the caller will handle it
            throw new BlUnknownTypeException("Error updating clock.", ex);
        }
    }

    #endregion Stage 4


    #region Stage 5

    internal static event Action? ClockUpdatedObservers; //prepared for stage 5 - for clock update observers

    #endregion Stage 5


    #region Stage 7 base
    internal static readonly object blMutex = new();
    private static Thread? s_thread;
    private static int s_interval { get; set; } = 1; //in minutes by second    
    private static volatile bool s_stop = false;
    private static object mutex = new();

    internal static void Start(int interval)
    {
        lock (mutex)
            if (s_thread == null)
            {
                s_interval = interval;
                s_stop = false;
                s_thread = new Thread(clockRunner);
                s_thread.Start();
            }
    }

    internal static void Stop()
    {
        lock (mutex)
            if (s_thread != null)
            {
                s_stop = true;
                s_thread?.Interrupt();
                s_thread = null;
            }
    }

    private static void clockRunner()
    {
        while (!s_stop)
        {
            UpdateClock(Now.AddMinutes(s_interval));

            #region Stage 7
            //TO_DO:
            //Add calls here to any logic simulation that was required in stage 7
            //for example: course registration simulation

            //StudentManager.SimulateCourseRegistrationAndGrade(); //stage 7

            //etc...
            #endregion Stage 7

            try
            {
                Thread.Sleep(1000); // 1 second
            }
            catch (ThreadInterruptedException) { }
        }
    }
    #endregion Stage 7 base
}