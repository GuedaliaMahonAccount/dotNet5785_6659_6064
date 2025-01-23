using BlImplementation;
using BO;
using DalApi;
using System.Runtime.CompilerServices;
namespace Helpers;

/// <summary>
/// Internal BL manager for all Application's Clock logic policies
/// </summary>
internal static class AdminManager //stage 4
{
    #region Stage 4
    private static readonly DalApi.IDal s_dal = DalApi.Factory.Get; //stage 4
    #endregion Stage 4

    #region Stage 5
    internal static event Action? ConfigUpdatedObservers; //prepared for stage 5 - for config update observers
    internal static event Action? ClockUpdatedObservers; //prepared for stage 5 - for clock update observers

    #endregion Stage 5

    #region Stage 4
    /// <summary>
    /// Property for providing/setting current configuration variable value for any BL class that may need it
    /// </summary>
    internal static TimeSpan RiskRange
    {
        get => s_dal.Config.RiskRange;
        set
        {
            s_dal.Config.RiskRange = value;

            // Update clock logic based on the new risk range
            updateClock(Now); // Pass the current clock value

            // Notify all observers about the configuration update
            ConfigUpdatedObservers?.Invoke();
        }
    }

    /// <summary>
    /// Property for providing current application's clock value for any BL class that may need it
    /// </summary>
    internal static DateTime Now { get => s_dal.Config.Clock; } //stage 4

    /// <summary>
    /// Method to perform application's clock from any BL class as may be required
    /// </summary>
    /// <param name="newClock">updated clock value</param>
    internal static void UpdateClock(DateTime newClock) //stage 4-7
    {
        // new Thread(() => { // stage 7 - not sure - still under investigation - see stage 7 instructions after it will be released        
        updateClock(newClock);//stage 4-6
        // }).Start(); // stage 7 as above
    }

    private static Task? _periodicTask = null;
    private static void updateClock(DateTime newClock) // Prepared for stage 7 as DRY to eliminate needless repetition
    {
        var oldClock = s_dal.Config.Clock; // Stage 4
        s_dal.Config.Clock = newClock; // Stage 4

        if (_periodicTask is null || _periodicTask.IsCompleted) // Stage 7
        {
            _periodicTask = Task.Run(() =>
            {
                CallManager.UpdateRiskCall(oldClock, newClock, RiskRange);
                CallManager.UpdateExpiredCalls();
            });
        }

        // Calling all the observers of clock update
        ClockUpdatedObservers?.Invoke(); // Prepared for stage 5
    }

    #endregion Stage 4

    #region Stage 7 base
    private static readonly object mutex = new();
    /// <summary>    
    /// Mutex to use from BL methods to get mutual exclusion while the simulator is running
    /// </summary>
    internal static readonly object BlMutex = new(); // BlMutex = s_dal; // This field is actually the same as s_dal - it is defined for readability of locks
    /// <summary>
    /// The thread of the simulator
    /// </summary>
    private static volatile Thread? s_thread;
    /// <summary>
    /// The Interval for clock updating
    /// in minutes by second (default value is 1, will be set on Start())    
    /// </summary>
    private static int s_interval { get; set; } = 1;
    /// <summary>
    /// The flag that signs whether simulator is running
    /// 
    private static volatile bool s_stop = false;


    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7                                                 
    public static void ThrowOnSimulatorIsRunning()
    {
        if (s_thread is not null)
            throw new BO.BLTemporaryNotAvailableException("Cannot perform the operation since Simulator is running");
    }

    internal static void ResetDB() //stage 7
    {
        lock (BlMutex)
        {
            s_dal.ResetDB();
            AdminManager.UpdateClock(AdminManager.Now);
            AdminManager.RiskRange = AdminManager.RiskRange;
        }
    }

    internal static void InitializeDB()
    {
        lock (BlMutex)
        {
            DalTest.Initialization.Do();
            AdminManager.UpdateClock(AdminManager.Now);
            AdminManager.RiskRange = AdminManager.RiskRange;
        }
    }



  


    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7                                                
    internal static void Start(int interval)
    {
        if (s_thread is null)
        {
            s_interval = interval;
            s_stop = false;
            s_thread = new(clockRunner) { Name = "ClockRunner" };
            s_thread.Start();
        }
    }


    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7                                                
    internal static void Stop()
    {
        if (s_thread is not null)
        {
            s_stop = true;
            s_thread.Interrupt(); //awake a sleeping thread
            s_thread.Name = "ClockRunner stopped";
            s_thread = null;
        }
    }
    private static Task? _simulateTask = null;

    private static void clockRunner()
    {
        while (!s_stop)
        {
            UpdateClock(Now.AddMinutes(s_interval));

            if (_simulateTask is null || _simulateTask.IsCompleted)//stage 7
                _simulateTask = Task.Run(() => CallManager.SimulateVolunteerActivity());
            try
            {
                Thread.Sleep(1000); // 1 second
            }
            catch (ThreadInterruptedException) { }
        }
    }







    #endregion Stage 7 base
}