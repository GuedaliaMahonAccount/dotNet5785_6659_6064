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

        public DateTime GetCurrentTime()
        {
            // No try-catch, throw the exception directly
            return AdminManager.Now;
        }

        public TimeSpan GetRiskTime()
        {
            // No try-catch, throw the exception directly
            return AdminManager.RiskRange;
        }

        public void InitializeDB() 
        {
            AdminManager.ThrowOnSimulatorIsRunning();  
            AdminManager.InitializeDB(); 
        }

        public void ResetDB() 
        {
            AdminManager.ThrowOnSimulatorIsRunning();  
            AdminManager.ResetDB(); 
        }


        public void SetRiskTime(TimeSpan riskTimeSpan)
        {
            AdminManager.RiskRange = riskTimeSpan;
        }


        ///<summary>
        /// observer
        /// </summary>
        public void AddClockObserver(Action clockObserver) =>
                    AdminManager.ClockUpdatedObservers += clockObserver;
        public void RemoveClockObserver(Action clockObserver) =>
                    AdminManager.ClockUpdatedObservers -= clockObserver;
        public void AddConfigObserver(Action configObserver) =>
                    AdminManager.ConfigUpdatedObservers += configObserver;
        public void RemoveConfigObserver(Action configObserver) =>
                    AdminManager.ConfigUpdatedObservers -= configObserver;

    }
}
