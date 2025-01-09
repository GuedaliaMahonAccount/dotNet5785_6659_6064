using PL.User;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace PL
{
    public partial class MainUserWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.Volunteer CurrentUser { get; set; }
        public ObservableCollection<BO.CallInProgress> CurrentCalls
        {
            get { return (ObservableCollection<BO.CallInProgress>)GetValue(CurrentCallsProperty); }
            set { SetValue(CurrentCallsProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallsProperty =
            DependencyProperty.Register("CurrentCalls", typeof(ObservableCollection<BO.CallInProgress>),
            typeof(MainUserWindow), new PropertyMetadata(new ObservableCollection<BO.CallInProgress>()));

        private readonly int _volunteerId;

        public MainUserWindow(int volunteerId)
        {
            InitializeComponent();
            _volunteerId = volunteerId;

            // Load user details
            CurrentUser = GetUserDetails(volunteerId);
            DataContext = this;

            // Load current calls
            QueryCurrentCalls();

            // Add observer to automatically refresh current calls
            s_bl.Call.AddObserver(CallObserver);
        }

        // Fetch user details based on the volunteer ID
        private BO.Volunteer GetUserDetails(int volunteerId)
        {
            return s_bl.Volunteer.GetVolunteerDetails(volunteerId);
        }

        // Query all current calls for the user
        private void QueryCurrentCalls()
        {
            try
            {
                // Fetch all current calls from the backend
                var calls = s_bl.Volunteer.GetCurrentCallsForVolunteer(_volunteerId);

                CurrentCalls.Clear(); // Clear the existing list
                foreach (var call in calls)
                {
                    CurrentCalls.Add(call); // Add each call to the ObservableCollection
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load current calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Observer to refresh current calls on UI updates
        private void CallObserver()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                QueryCurrentCalls(); // Refresh current calls on the UI thread
            });
        }

        public ICommand CompleteCallCommand => new RelayCommand(CompleteCall);
        public ICommand CancelCallCommand => new RelayCommand(CancelCall);

        // Adjust methods to handle the `object` parameter
        private void CompleteCall(object parameter)
        {
            if (parameter is BO.CallInProgress call)
            {
                try
                {
                    int assignmentId = s_bl.Call.GetAssignmentIdByCallId(call.CallId, CurrentUser.Id);
                    s_bl.Call.CompleteCall(CurrentUser.Id, assignmentId);
                    MessageBox.Show("Call completed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    QueryCurrentCalls();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to complete call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void CancelCall(object parameter)
        {
            if (parameter is BO.CallInProgress call)
            {
                try
                {
                    int assignmentId = s_bl.Call.GetAssignmentIdByCallId(call.CallId, CurrentUser.Id);
                    s_bl.Call.CancelCall(CurrentUser.Id, assignmentId);
                    MessageBox.Show("Call canceled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    QueryCurrentCalls();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to cancel call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        // Open the history window
        private void ViewHistory_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new HistoryCallWindow(CurrentUser.Name);
            historyWindow.Show();
        }

        // Open the call selection window
        private void ChooseCall_Click(object sender, RoutedEventArgs e)
        {
            var choiceWindow = new ChoiceCallWindow(CurrentUser.Id);
            choiceWindow.Show();
        }

        // Cleanup when the window is closed
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            s_bl.Call.RemoveObserver(CallObserver); // Remove observer to prevent memory leaks
        }
    }
}
