using System.Windows;
using BO;

namespace PL.Volunteer
{
    public partial class CurrentCallWindow : Window
    {
        public CallInProgress? CurrentCall { get; }

        public CurrentCallWindow(CallInProgress? currentCall)
        {
            // Filter the call by status: InTreatment or InTreatmentAtRisk
            CurrentCall = (currentCall != null &&
                          (currentCall.CallType == CallType.InTreatment || currentCall.CallType == CallType.InTreatmentAtRisk))
                          ? currentCall
                          : null;

            // Set DataContext to the filtered call or a placeholder
            DataContext = CurrentCall ?? new CallInProgress
            {
                GeneralDescription = "No relevant call available."
            };

            InitializeComponent();
        }
    }
}
