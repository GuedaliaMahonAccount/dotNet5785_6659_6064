using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BO;

namespace PL.User
{
    public partial class HistoryCallWindow : Window, INotifyPropertyChanged
    {
        // Static reference to business logic layer
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // Dependency property for list of calls displayed in the DataGrid
        public IEnumerable<CallInList> Calls
        {
            get { return (IEnumerable<CallInList>)GetValue(CallsProperty); }
            set { SetValue(CallsProperty, value); OnPropertyChanged(nameof(Calls)); }
        }

        public static readonly DependencyProperty CallsProperty =
            DependencyProperty.Register("Calls", typeof(IEnumerable<CallInList>),
                typeof(HistoryCallWindow), new PropertyMetadata(null));

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public HistoryCallWindow(string volunteerName)
        {
            InitializeComponent();
            this.DataContext = this; // הגדרת ה-DataContext

            int volunteerId = s_bl.Volunteer.FindVolunteerID(volunteerName);
            VolunteerIdTextBlock.Text = volunteerId.ToString();
            VolunteerNameTextBlock.Text = volunteerName;

            LoadCalls(volunteerId); // טעינת הנתונים לאחר אתחול החלון
        }

        /// <summary>
        /// Loads the call list for a specific volunteer into the DataGrid.
        /// </summary>
        private void LoadCalls(int volunteerId)
        {
            try
            {
                var callDetails = s_bl.Call.CallHistoryByVolunteerId(volunteerId);

                if (callDetails == null || !callDetails.Any())
                {
                    MessageBox.Show("No calls found for the specified volunteer.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    CallsDataGrid.ItemsSource = callDetails.Select(call => new
                    {
                        Id = call.Id,
                        StartTime = call.StartTime,
                        Description = call.Description,
                        Status = call.CallType
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Filters the call list (basic functionality).
        /// </summary>
        private void ApplyFilter()
        {
            // Example: Reload calls (replace with actual filter logic if needed)
            int volunteerId = int.Parse(VolunteerIdTextBlock.Text);
            LoadCalls(volunteerId);
        }

        /// <summary>
        /// Handler for the Filter button click event.
        /// </summary>
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        /// <summary>
        /// Handler for when the DataGrid selection changes.
        /// </summary>
        private void CallsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CallsDataGrid.SelectedItem is CallInList selectedCall)
            {
                MessageBox.Show($"Selected Call ID: {selectedCall.CallId}\nDate: {selectedCall.StartTime}\nStatus: {selectedCall.Status}");
            }
        }
    }
}