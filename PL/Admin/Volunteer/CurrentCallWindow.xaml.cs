using System.Windows;
using BO;

namespace PL.Volunteer
{
    public partial class CurrentCallWindow : Window
    {
        public CallInProgress? CurrentCall { get; }

        public CurrentCallWindow(CallInProgress? currentCall)
        {
            CurrentCall = currentCall;
            DataContext = CurrentCall ?? new CallInProgress
            {
                GeneralDescription = "No current call available."
            };
            InitializeComponent();
        }
    }

}
