using System;
using System.Collections.Generic;

namespace BlApi
{
    public interface IAdmin
    {
        DateTime GetCurrentTime();
        void AdvanceClock(BO.TimeUnit timeUnit);
        TimeSpan GetRiskTime();
        void SetRiskTime(TimeSpan riskTimeSpan);
        void ResetDatabase();
        void InitializeDatabase();
    }
}
