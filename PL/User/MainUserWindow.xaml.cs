using PL.User;
using System.Windows;
using System.Windows.Input;

namespace PL
{
    public partial class MainUserWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.Volunteer CurrentUser { get; set; }
        public BO.CallInProgress CurrentCall
        {
            get { return (BO.CallInProgress)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.CallInProgress),
            typeof(MainUserWindow), new PropertyMetadata(null));

        private readonly int _volunteerId;

        public MainUserWindow(int volunteerId)
        {
            InitializeComponent();
            _volunteerId = volunteerId;

            CurrentUser = GetUserDetails(volunteerId);

            DataContext = this;

            // Initial query to load the current call
            QueryCurrentCall();

            // Add an observer to automatically refresh the current call
            s_bl.Call.AddObserver(CallObserver);
        }

        private BO.Volunteer GetUserDetails(int volunteerId)
        {
            return s_bl.Volunteer.GetVolunteerDetails(volunteerId);
        }

        private void QueryCurrentCall()
        {
            try
            {
                CurrentCall = s_bl.Volunteer.GetVolunteerDetails(_volunteerId).CurrentCall;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load current call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CallObserver()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                QueryCurrentCall(); // Refresh the current call on the UI thread
            });
        }

        private void CompleteCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int assignmentId = s_bl.Call.GetAssignmentIdByCallId(CurrentCall.CallId, CurrentUser.Id);
                s_bl.Call.CompleteCall(CurrentUser.Id, assignmentId);

                MessageBox.Show("Call completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Update to the next assignment
                UpdateToNextAssignment();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to complete call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int assignmentId = s_bl.Call.GetAssignmentIdByCallId(CurrentCall.CallId, CurrentUser.Id);
                s_bl.Call.CancelCall(CurrentUser.Id, assignmentId);

                MessageBox.Show("Call canceled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        
        // Update to the next assignment
                UpdateToNextAssignment();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to cancel call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateToNextAssignment()
        {
            try
            {
                // Fetch updated user details
                var updatedUser = s_bl.Volunteer.GetVolunteerDetails(_volunteerId);

                // Update CurrentCall to the next available assignment, if any
                CurrentCall = updatedUser.CurrentCall;

                if (CurrentCall == null)
                {
                    MessageBox.Show("No more assignments available.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update to next assignment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            s_bl.Call.RemoveObserver(CallObserver);
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
