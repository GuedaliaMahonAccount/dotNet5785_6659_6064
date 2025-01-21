
namespace BlApi
{
    public interface IAdmin
    {
        DateTime GetCurrentTime();
        void UpdateClock(BO.TimeUnit timeUnit);
        TimeSpan GetRiskTime();
        void SetRiskTime(TimeSpan riskTimeSpan);
        void ResetDB();
        void InitializeDB();


        #region Stage 5
        void AddConfigObserver(Action configObserver);
        void RemoveConfigObserver(Action configObserver);
        void AddClockObserver(Action clockObserver);
        void RemoveClockObserver(Action clockObserver);
        #endregion Stage 5
    }
}
