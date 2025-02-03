using PL.User;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using BO;
using PL.Volunteer;
using System.ComponentModel;

namespace PL
{
    public partial class MainUserWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private BO.Volunteer _currentUser;
        public BO.Volunteer CurrentUser
        {
            get { return _currentUser; }
            set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    OnPropertyChanged(nameof(CurrentUser)); // event PropertyChanged
                }
            }
        }



        public ObservableCollection<BO.CallInProgress> CurrentCalls
        {
            get { return (ObservableCollection<BO.CallInProgress>)GetValue(CurrentCallsProperty); }
            set { SetValue(CurrentCallsProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallsProperty =
            DependencyProperty.Register("CurrentCalls", typeof(ObservableCollection<BO.CallInProgress>),
            typeof(MainUserWindow), new PropertyMetadata(new ObservableCollection<BO.CallInProgress>()));

        private readonly int _volunteerId;

        // Flag to prevent multiple updates
        private volatile bool _isUpdating = false;

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
            if (!_isUpdating)
            {
                _isUpdating = true;
                Dispatcher.BeginInvoke(new Action(() =>
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
                    finally
                    {
                        _isUpdating = false; // Reset the flag after the operation is done
                    }
                }));
            }
        }

        // Observer to refresh current calls on UI updates
        private void CallObserver()
        {
            if (!_isUpdating)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    QueryCurrentCalls(); // Refresh current calls on the UI thread
                    _isUpdating = false; // Reset the flag after the operation is done
                }));
            }
        }

        public ICommand CompleteCallCommand => new User.RelayCommand(CompleteCall);
        public ICommand CancelCallCommand => new User.RelayCommand(CancelCall);

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
            try
            {
                s_bl.Admin.checkSimulator();
                var historyWindow = new HistoryCallWindow(CurrentUser.Name);
                historyWindow.Show();
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void UpdateVolunteer_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser != null)
            {
                var volunteerWindow = new VolunteerWindow(CurrentUser.Id);
                volunteerWindow.VolunteerUpdated += (updatedVolunteer) =>
                {
                    CurrentUser = updatedVolunteer;
                };
                volunteerWindow.Show();
            }
        }

        // Implement INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}