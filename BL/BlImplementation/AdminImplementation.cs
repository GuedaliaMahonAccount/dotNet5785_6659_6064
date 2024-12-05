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
            try
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
            catch (Exception ex)
            {
                throw new BO.LogicException($"Error updating clock: {ex.Message}", ex);
            }
        }

        public DateTime GetCurrentTime()
        {
            try
            {
                return ClockManager.Now;
            }
            catch (Exception ex)
            {
                throw new BO.LogicException($"Error retrieving current time: {ex.Message}", ex);
            }
        }

        public TimeSpan GetRiskTime()
        {
            try
            {
                return _dal.Config.RiskRange;
            }
            catch (Exception ex)
            {
                throw new BO.LogicException($"Error retrieving risk time: {ex.Message}", ex);
            }
        }

        public void InitializeDatabase()
        {
            try
            {
                ResetDatabase();
                DalTest.Initialization.Do();
                ClockManager.UpdateClock(ClockManager.Now);
            }
            catch (Exception ex)
            {
                throw new BO.LogicException($"Error initializing database: {ex.Message}", ex);
            }
        }

        public void ResetDatabase()
        {
            try
            {
                _dal.ResetDB();
            }
            catch (Exception ex)
            {
                throw new BO.LogicException($"Error resetting database: {ex.Message}", ex);
            }
        }

        public void SetRiskTime(TimeSpan riskTimeSpan)
        {
            try
            {
                _dal.Config.RiskRange = riskTimeSpan;
            }
            catch (Exception ex)
            {
                throw new BO.LogicException($"Error setting risk time: {ex.Message}", ex);
            }
        }
    }
}
