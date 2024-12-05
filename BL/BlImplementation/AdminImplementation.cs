using BlApi;
using BO;


namespace BlImplementation
{
    internal class AdminImplementation : IAdmin
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        public void UpdateClock(TimeUnit timeUnit)
        {
            throw new NotImplementedException();
        }

        public DateTime GetCurrentTime()
        {
            throw new NotImplementedException();
        }

        public TimeSpan GetRiskTime()
        {
            throw new NotImplementedException();
        }

        public void InitializeDatabase()
        {
            throw new NotImplementedException();
        }

        public void ResetDatabase()
        {
            throw new NotImplementedException();
        }

        public void SetRiskTime(TimeSpan riskTimeSpan)
        {
            throw new NotImplementedException();
        }
    }
}
