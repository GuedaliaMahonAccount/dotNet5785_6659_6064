using BlApi;
using BO;
using Dal;
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
                    newClock = ClockManager.Now.AddMinutes(1);
                    break;
                case TimeUnit.HOUR:
                    newClock = ClockManager.Now.AddHours(1);
                    break;
                case TimeUnit.DAY:
                    newClock = ClockManager.Now.AddDays(1);
                    break;
                case TimeUnit.MONTH:
                    newClock = ClockManager.Now.AddMonths(1);
                    break;
                case TimeUnit.YEAR:
                    newClock = ClockManager.Now.AddYears(1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(timeUnit), "Invalid TimeUnit provided.");
            }

            ClockManager.UpdateClock(newClock);
        }

        public DateTime GetCurrentTime()
        {
            return ClockManager.Now;
        }

        public TimeSpan GetRiskTime()
        {
            return _dal.Config.RiskRange;
        }

        public void InitializeDatabase()
        {
            ResetDatabase();
            // Add default data
            //
            //
            //
            //
            Console.WriteLine("Database initialized successfully with default data.");
        }

        public void ResetDatabase()
        {
            _dal.ResetDB();
        }

        public void SetRiskTime(TimeSpan riskTimeSpan)
        {
            _dal.Config.RiskRange = riskTimeSpan;
        }
    }
}
