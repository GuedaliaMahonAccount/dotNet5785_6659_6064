using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
