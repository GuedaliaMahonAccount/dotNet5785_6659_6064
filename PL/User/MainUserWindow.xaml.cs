using PL.User;
using System.Windows;

namespace PL
{
    public partial class MainUserWindow : Window
    {
        public BO.Volunteer CurrentUser { get; set; }
        public BO.CallInProgress CurrentCall { get; set; }

        public MainUserWindow(int volunteerId)
        {
            CurrentUser = GetUserDetails(volunteerId);
            CurrentCall = GetCurrentCall(volunteerId);

            DataContext = this;
            InitializeComponent();
        }

        private BO.Volunteer GetUserDetails(int volunteerId)
        {
            // Fetch volunteer details from BL using volunteerId
            return BlApi.Factory.Get().Volunteer.GetVolunteerDetails(volunteerId);
        }

        private BO.CallInProgress GetCurrentCall(int volunteerId)
        {
            // Fetch the current call for the volunteer from BL using volunteerId
            return BlApi.Factory.Get().Volunteer.GetVolunteerDetails(volunteerId).CurrentCall;
        }

        private void CompleteCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BlApi.Factory.Get().Call.CompleteCall(CurrentUser.Id , CurrentCall.CallId);
                MessageBox.Show("Call completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CurrentCall = null; // Clear current call
                DataContext = this; // Refresh binding
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error completing call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BlApi.Factory.Get().Call.CancelCall(CurrentUser.Id , CurrentCall.CallId);
                MessageBox.Show("Call canceled.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                CurrentCall = null; // Clear current call
                DataContext = this; // Refresh binding
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error canceling call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewHistory_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new HistoryCallWindow(CurrentUser.Name);
            historyWindow.Show();
        }

        private void ChooseCall_Click(object sender, RoutedEventArgs e)
        {
            var choiceWindow = new ChoiceCallWindow(CurrentUser.Id);
            choiceWindow.Show();
        }
    }
}
