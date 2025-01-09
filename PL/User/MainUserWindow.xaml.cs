using BO;
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
                // Fetch the AssignmentId for the current call and volunteer
                int assignmentId = BlApi.Factory.Get().Call.GetAssignmentIdByCallId(CurrentCall.CallId, CurrentUser.Id);

                // Use the AssignmentId to complete the call
                BlApi.Factory.Get().Call.CompleteCall(CurrentUser.Id, assignmentId);

                MessageBox.Show("Call completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                CurrentCall = null; // Clear the current call
                DataContext = this; // Refresh the binding
            }
            catch (BlDoesNotExistException ex)
            {
                MessageBox.Show($"Call not found: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BlInvalidRoleException ex)
            {
                MessageBox.Show($"You are not authorized to complete this call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BlInvalidValueException ex)
            {
                MessageBox.Show($"Invalid operation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                // Fetch the AssignmentId for the current call and volunteer
                int assignmentId = BlApi.Factory.Get().Call.GetAssignmentIdByCallId(CurrentCall.CallId, CurrentUser.Id);

                // Use the AssignmentId to cancel the call
                BlApi.Factory.Get().Call.CancelCall(CurrentUser.Id, assignmentId);
  

                MessageBox.Show("Call canceled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                CurrentCall = null; // Clear the current call
                DataContext = this; // Refresh the binding
            }
            catch (BlDoesNotExistException ex)
            {
                MessageBox.Show($"Call not found: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BlInvalidRoleException ex)
            {
                MessageBox.Show($"You are not authorized to cancel this call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BlInvalidValueException ex)
            {
                MessageBox.Show($"Invalid operation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
