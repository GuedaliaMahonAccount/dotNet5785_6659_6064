using BlApi;
using BO;
using Helpers;

namespace BlImplementation
{
    internal class AdminImplementation : IAdmin
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        public void UpdateClock(TimeUnit timeUnit)
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            lock (AdminManager.BlMutex)
            {
                DateTime newClock;

                switch (timeUnit)
                {
                    case TimeUnit.MINUTE:
                        newClock = AdminManager.Now.AddMinutes(1);
                        break;
                    case TimeUnit.HOUR:
                        newClock = AdminManager.Now.AddHours(1);
                        break;
                    case TimeUnit.DAY:
                        newClock = AdminManager.Now.AddDays(1);
                        break;
                    case TimeUnit.MONTH:
                        newClock = AdminManager.Now.AddMonths(1);
                        break;
                    case TimeUnit.YEAR:
                        newClock = AdminManager.Now.AddYears(1);
                        break;
                    default:
                        // No try-catch, throw the exception directly
                        throw new BlInvalidValueException($"Invalid TimeUnit provided: {timeUnit}");
                }

                AdminManager.UpdateClock(newClock);
            }
        }

        public DateTime GetCurrentTime()
        {
            lock (AdminManager.BlMutex)
                return AdminManager.Now;
        }

        public TimeSpan GetRiskTime()
        {
            lock (AdminManager.BlMutex)
                return AdminManager.RiskRange;
        }

        public void InitializeDB() 
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            lock (AdminManager.BlMutex)
                AdminManager.InitializeDB();
        }

        public void ResetDB() 
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            lock (AdminManager.BlMutex)
                AdminManager.ResetDB();
        }


        public void SetRiskTime(TimeSpan riskTimeSpan)
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            lock (AdminManager.BlMutex)
                AdminManager.RiskRange = riskTimeSpan;
        }


        ///<summary>
        /// observer
        /// </summary>
        public void AddClockObserver(Action clockObserver)
        {
            lock (AdminManager.BlMutex)
                AdminManager.ClockUpdatedObservers += clockObserver;
        }

        public void RemoveClockObserver(Action clockObserver)
        {
            lock (AdminManager.BlMutex)
                AdminManager.ClockUpdatedObservers -= clockObserver;
        }

        public void AddConfigObserver(Action configObserver)
        {
            lock (AdminManager.BlMutex)
                AdminManager.ConfigUpdatedObservers += configObserver;
        }

        public void RemoveConfigObserver(Action configObserver)
        {
            lock (AdminManager.BlMutex)
                AdminManager.ConfigUpdatedObservers -= configObserver;
        }


        public void StartSimulator(int interval)  
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            lock (AdminManager.BlMutex)
                            AdminManager.Start(interval);
            
        }

        public void StopSimulator()
        {
            lock (AdminManager.BlMutex)
                AdminManager.Stop();
        }

        public void checkSimulator()
        {
            AdminManager.ThrowOnSimulatorIsRunning() ;
        }

    }
}
