using BO;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Helpers;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow : Window
    {
        // Static reference to business logic layer
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // Observable collection for binding the list of calls
        public ObservableCollection<BO.CallInList> CallList { get; set; } = new ObservableCollection<BO.CallInList>();

        // Collection for call type filter
        public ObservableCollection<BO.Status> CallTypeCollection { get; } = new ObservableCollection<BO.Status>((BO.Status[])Enum.GetValues(typeof(BO.Status)));

        // Property for the selected call type filter
        public BO.Status Type { get; set; }

        // Property for tracking the selected call in the list
        public BO.CallInList? SelectedCall { get; set; }

        // Flag to prevent multiple updates
        private volatile bool _isUpdating = false;

        /// <summary>
        /// Default constructor - initializes the window with no filter (CallType.None)
        /// </summary>
        public CallListWindow() : this(BO.Status.None) { }

        /// <summary>
        /// Constructor - initializes the window with a specific call type filter
        /// </summary>
        /// <param name="status">The call type to filter the list by</param>
        public CallListWindow(BO.Status status)
        {
            InitializeComponent();
            Type = status;
            DataContext = this; // Bind the data context to the current instance
            queryCallList();    // Load the initial call list
        }

        /// <summary>
        /// Event handler for the call type filter ComboBox selection change
        /// </summary>
        private void CbCallTypeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryCallList();
        }

        /// <summary>
        /// Queries and updates the list of calls based on the selected call type filter
        /// </summary>
        private void queryCallList()
        {
            if (!_isUpdating)
            {
                _isUpdating = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        CallList.Clear();
                        var filteredCalls = Type == BO.Status.None
                            ? s_bl.Call.GetCallList()
                            : s_bl.Call.GetCallList(Type);

                        foreach (var call in filteredCalls)
                        {
                            CallList.Add(call);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading call list: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        _isUpdating = false; // Reset the flag after the operation is done
                    }
                }));
            }
        }

        /// <summary>
        /// Event handler for the window's Loaded event - adds an observer for refreshing the call list
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(queryCallList);
        }

        /// <summary>
        /// Event handler for the window's Closed event - removes the observer to avoid memory leaks
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(queryCallList);
        }

        /// <summary>
        /// Event handler for double-clicking an item in the call list - opens the CallWindow for the selected call
        /// </summary>
        private void lsvCallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall?.CallId != null)
            {
                new CallWindow(SelectedCall.CallId.Value).Show();
            }
            else
            {
                MessageBox.Show("Selected call does not have a valid ID.");
            }
        }

        /// <summary>
        /// Event handler for clicking the "Add Call" button - opens a new CallWindow for adding a call
        /// </summary>
        private void AddCallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Call the method to check if the simulator is running
                s_bl.Admin.checkSimulator();

                // If no exception, open the CallWindow
                new CallWindow().Show();
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                // Catch the specific exception and show an error message
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Catch any other unexpected exceptions
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Event handler for the Delete button in the DataGrid
        /// </summary>
        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int callId)
            {
                try
                {
                    // Confirm deletion
                    var result = MessageBox.Show($"Are you sure you want to delete call {callId}?", "Confirm Deletion",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        s_bl.Call.DeleteCall(callId);
                        queryCallList(); // Refresh the call list
                    }
                }
                catch (BO.BlInvalidValueException ex)
                {
                    MessageBox.Show($"Cannot delete this call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.BlDeletionImpossibleException ex)
                {
                    MessageBox.Show($"This call cannot be deleted: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Event handler for the Cancel (by Admin) button in the DataGrid
        /// </summary>
        private void CancelCall_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int callId)
            {
                try
                {
                    int adminId = s_bl.Volunteer.GetAdminId(); // Fetch admin ID dynamically

                    // Confirm cancellation
                    var result = MessageBox.Show($"Are you sure you want to cancel call {callId}?", "Confirm Cancellation",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        s_bl.Call.CancelCall(adminId, callId);
                        queryCallList(); // Refresh the call list
                    }
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.BlInvalidRoleException ex)
                {
                    MessageBox.Show($"Permission Denied: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.BlInvalidValueException ex)
                {
                    MessageBox.Show($"Invalid operation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }

    public class TimeSpanToLabeledStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                if (timeSpan == TimeSpan.Zero)
                {
                    return "No time left";
                }

                var parts = new List<string>();

                if (timeSpan.Days > 0)
                    parts.Add($"{timeSpan.Days} days");
                if (timeSpan.Hours > 0)
                    parts.Add($"{timeSpan.Hours} hours");
                if (timeSpan.Minutes > 0)
                    parts.Add($"{timeSpan.Minutes} minutes");
                if (timeSpan.Seconds > 0)
                    parts.Add($"{timeSpan.Seconds} seconds");

                return parts.Count > 0 ? string.Join(", ", parts) : "0";
            }

            return "No time left"; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CallTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BO.Status status)
            {
                return status switch
                {
                    BO.Status.None => Brushes.Gray,
                    BO.Status.Open => Brushes.Green,
                    BO.Status.InTreatment => Brushes.Blue,
                    BO.Status.Completed => Brushes.DarkGreen,
                    BO.Status.Expired => Brushes.OrangeRed,
                    BO.Status.SelfCanceled => Brushes.Orange,
                    BO.Status.AdminCanceled => Brushes.Red,
                    BO.Status.OpenAtRisk => Brushes.Yellow,
                    BO.Status.InTreatmentAtRisk => Brushes.Gold,
                    _ => Brushes.Black
                };
            }

            return Brushes.Black; // Default color for unknown types
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}