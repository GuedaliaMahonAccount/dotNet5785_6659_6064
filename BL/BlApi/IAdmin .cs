
namespace BlApi
{
    public interface IAdmin
    {
        DateTime GetCurrentTime();
        void UpdateClock(BO.TimeUnit timeUnit);
        TimeSpan GetRiskTime();
        void SetRiskTime(TimeSpan riskTimeSpan);
        void ResetDatabase();
        void InitializeDatabase();
    }
}
