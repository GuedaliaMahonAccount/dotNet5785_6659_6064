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
        private IEnumerable<object> _callDetails;

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
            this.DataContext = this;

            int volunteerId = s_bl.Volunteer.FindVolunteerID(volunteerName);
            VolunteerIdTextBlock.Text = volunteerId.ToString();
            VolunteerNameTextBlock.Text = volunteerName;

            LoadCalls(volunteerId);
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
                    _callDetails = callDetails.Select(call => new
                    {
                        Id = call.Id,
                        StartTime = call.StartTime,
                        Description = call.Description,
                        Status = call.CallType
                    });

                    CallsDataGrid.ItemsSource = _callDetails;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Filters the call list based on user selection.
        /// </summary>
        private void ApplyFilter()
        {
            // Ask the user what they want to filter by
            var filterOption = MessageBox.Show("Filter by:\nYes - Date\nNo - Call Type", "Filter Options", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (filterOption == MessageBoxResult.Yes)
            {
                // Filter by date
                FilterByDate();
            }
            else if (filterOption == MessageBoxResult.No)
            {
                // Filter by call type
                FilterByCallType();
            }
        }

        /// <summary>
        /// Filters the call list by date.
        /// </summary>
        private void FilterByDate()
        {
            var datePickerDialog = new Window
            {
                Title = "Filter by Date",
                Width = 300,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            var datePicker = new DatePicker
            {
                SelectedDate = DateTime.Now
            };

            var okButton = new Button
            {
                Content = "OK",
                Width = 100,
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            okButton.Click += (sender, e) =>
            {
                var selectedDate = datePicker.SelectedDate;
                if (selectedDate.HasValue)
                {
                    var filteredCalls = _callDetails.Where(call => ((dynamic)call).StartTime.Date == selectedDate.Value.Date).ToList();
                    CallsDataGrid.ItemsSource = filteredCalls;
                }
                else
                {
                    MessageBox.Show("Invalid date selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                datePickerDialog.Close();
            };

            stackPanel.Children.Add(datePicker);
            stackPanel.Children.Add(okButton);
            datePickerDialog.Content = stackPanel;

            datePickerDialog.ShowDialog();
        }

        /// <summary>
        /// Filters the call list by call type.
        /// </summary>
        private void FilterByCallType()
        {
            var callTypes = Enum.GetNames(typeof(CallType)).ToList(); // Get all call type names as strings
            callTypes.Insert(0, "none"); // Add "none" option at the beginning of the list

            // Create a custom dialog to let the user choose a call type
            var dialog = new Window
            {
                Title = "Filter by Call Type",
                Width = 300,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };
            var comboBox = new ComboBox
            {
                ItemsSource = callTypes,
                SelectedIndex = 0 // Default selection
            };

            var okButton = new Button
            {
                Content = "OK",
                Width = 100,
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            okButton.Click += (sender, e) =>
            {
                var selectedCallTypeString = comboBox.SelectedItem as string;
                if (selectedCallTypeString == "none")
                {
                    // If "none" is selected, reset the filter to show all calls
                    CallsDataGrid.ItemsSource = _callDetails;
                }
                else if (!string.IsNullOrEmpty(selectedCallTypeString) && Enum.TryParse(selectedCallTypeString, out CallType selectedCallType))
                {
                    // Filter by the selected call type
                    var filteredCalls = _callDetails.Where(call => ((dynamic)call).Status == selectedCallType).ToList();
                    CallsDataGrid.ItemsSource = filteredCalls;
                }
                else
                {
                    MessageBox.Show("Invalid call type selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                dialog.Close();
            };

            stackPanel.Children.Add(comboBox);
            stackPanel.Children.Add(okButton);
            dialog.Content = stackPanel;

            dialog.ShowDialog(); // Show the dialog
        }


        /// <summary>
        /// Handler for the Filter button click event.
        /// </summary>
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        /// <summary>
        /// Resets the filter and displays all calls.
        /// </summary>
        private void ResetFilter()
        {
            CallsDataGrid.ItemsSource = _callDetails;
        }

        /// <summary>
        /// Handler for the Reset Filter button click event.
        /// </summary>
        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            ResetFilter();
        }
    }
}