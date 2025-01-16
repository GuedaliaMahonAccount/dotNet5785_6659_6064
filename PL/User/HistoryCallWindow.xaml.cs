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
        /// Filters the call list by call type.
        /// </summary>
        /// <summary>
        /// Filters the call list by call type.
        /// </summary>
        private void FilterByCallType()
        {
            var callTypes = Enum.GetNames(typeof(CallType)).ToList(); // Get all call type names as strings

            // Create a custom dialog to let the user choose a call type
            var dialog = new Window
            {
                Title = "Filter by Call Type",
                Width = 300,
                Height = 250,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            var stackPanel = new StackPanel { Margin = new Thickness(10) };

            // Add a title TextBlock
            var titleTextBlock = new TextBlock
            {
                Text = "Filter by Call Type",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var comboBox = new ComboBox
            {
                ItemsSource = callTypes,
                SelectedIndex = 0 
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

                if (!string.IsNullOrEmpty(selectedCallTypeString))
                {
                    if (selectedCallTypeString == "None")
                    {
                        // No filtering, display all calls
                        CallsDataGrid.ItemsSource = _callDetails;
                    }
                    else if (Enum.TryParse(selectedCallTypeString, out CallType selectedCallType))
                    {
                        // Filter by the selected call type
                        var filteredCalls = _callDetails.Where(call => ((dynamic)call).Status == selectedCallType).ToList();
                        CallsDataGrid.ItemsSource = filteredCalls;
                    }
                    else
                    {
                        MessageBox.Show("Invalid call type selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                dialog.Close();
            };

            stackPanel.Children.Add(titleTextBlock);
            stackPanel.Children.Add(comboBox);
            stackPanel.Children.Add(okButton);

            dialog.Content = stackPanel;

            dialog.ShowDialog();
        }




        /// <summary>
        /// Handler for the Filter button click event.
        /// </summary>
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterByCallType();
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            ResetFilter();
        }

        /// <summary>
        /// Resets the filter and displays all calls.
        /// </summary>
        private void ResetFilter()
        {
            CallsDataGrid.ItemsSource = _callDetails;
        }
    }
}
