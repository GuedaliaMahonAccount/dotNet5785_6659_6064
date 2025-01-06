using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.User
{
    /// <summary>
    /// Interaction logic for HistoryCallWindow.xaml
    /// </summary>
    public partial class HistoryCallWindow : Window
    {
            // Static reference to business logic layer
            static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

            // Dependency property for list of calls displayed in the DataGrid
            public IEnumerable<BO.CallInList> Calls
            {
                get { return (IEnumerable<BO.CallInList>)GetValue(CallsProperty); }
                set { SetValue(CallsProperty, value); }
            }

            public static readonly DependencyProperty CallsProperty =
                DependencyProperty.Register("Calls", typeof(IEnumerable<BO.CallInList>),
                    typeof(HistoryCallWindow), new PropertyMetadata(null));

            public HistoryCallWindow(string volunteerName)
            {
                InitializeComponent();
            int volunteerId=s_bl.Volunteer.FindVolunteerID(volunteerName);
            // Set volunteer information
            VolunteerIdTextBlock.Text = volunteerId.ToString();
                VolunteerNameTextBlock.Text = volunteerName;

                // Load calls for the specific volunteer
                LoadCalls(volunteerId);
            }

            /// <summary>
            /// Loads the call list for a specific volunteer into the DataGrid.
            /// </summary>
            //private void LoadCalls(int volunteerId)
            //{
            //    Calls = s_bl.Call.GetCallHistoryByVolunteerId(volunteerId);
            //}

        /// <summary>
        /// Loads the call list for a specific volunteer into the DataGrid.
        /// </summary>
        private void LoadCalls(int volunteerId)
        {
            try
            {
                var calls = s_bl.Call.GetCallHistoryByVolunteerId(volunteerId);
                if (calls == null)
                {
                    MessageBox.Show("No calls found for the specified volunteer.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading calls: {ex.Message}");
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
                if (CallsDataGrid.SelectedItem is BO.CallInList selectedCall)
                {
                    MessageBox.Show($"Selected Call ID: {selectedCall.CallId}\nDate: {selectedCall.StartTime}\nStatus: {selectedCall.Status}");
                }
            }
        }
    }






