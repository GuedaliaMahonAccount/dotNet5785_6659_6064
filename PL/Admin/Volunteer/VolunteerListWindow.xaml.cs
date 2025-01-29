using BO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PL.Volunteer
{
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private IEnumerable<BO.VolunteerInList> _allVolunteers;
        private string _currentNameFilter = string.Empty; 
        private BO.CallType _currentStatusFilter = BO.CallType.None; 

        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerInListProperty); }
            set { SetValue(VolunteerInListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerInListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>),
            typeof(VolunteerListWindow), new PropertyMetadata(null));

        public BO.CallType Type { get; set; } = BO.CallType.None;

        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        // Flag to prevent multiple updates
        private volatile bool _isUpdating = false;

        public VolunteerListWindow()
        {
            InitializeComponent();
            _allVolunteers = s_bl.Volunteer.GetVolunteersList(); 
            VolunteerList = _allVolunteers; 
            _currentNameFilter = string.Empty; 
            _currentStatusFilter = BO.CallType.None; 
        }

        private bool _isWindowLoaded = false;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _isWindowLoaded = true;
            s_bl.Volunteer.AddObserver(() =>
            {
                if (!_isUpdating)
                {
                    _isUpdating = true;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            _allVolunteers = s_bl.Volunteer.GetVolunteersList();

                            var filteredList = _allVolunteers;

                            if (!string.IsNullOrEmpty(_currentNameFilter))
                            {
                                filteredList = filteredList.Where(volunteer =>
                                    volunteer.Name.IndexOf(_currentNameFilter, StringComparison.OrdinalIgnoreCase) >= 0);
                            }

                            if (_currentStatusFilter != BO.CallType.None)
                            {
                                filteredList = filteredList.Where(volunteer =>
                                    volunteer.CurrentCallType == _currentStatusFilter);
                            }

                            VolunteerList = filteredList.ToList();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error updating volunteer list: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            _isUpdating = false;
                        }
                    }));
                }
            });
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(() => VolunteerList = s_bl.Volunteer.GetVolunteersList());
        }

        private void lsvVolunteerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
                new VolunteerWindow(SelectedVolunteer.Id).Show();
        }

        private void AddvolunteerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // If you want to perform any checks before opening the VolunteerWindow, 
                // like checking if the simulator is running, you can do so here.

                // Example: Check if the simulator is running before opening the VolunteerWindow
                s_bl.Admin.checkSimulator();

                // If no exception, open the VolunteerWindow
                new VolunteerWindow().Show();
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


        private void DeleteVolunteer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int volunteerId)
            {
                if (volunteerId == 322766064)
                {
                    MessageBox.Show("The admin cannot be deleted",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this volunteer?",
                                             "Confirm Delete",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Volunteer.DeleteVolunteer(volunteerId);
                        // Observer pattern will refresh the list
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting volunteer: {ex.Message}",
                                        "Error",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CbVolunteerTypeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedValue is BO.CallType selectedStatus)
            {
                _currentStatusFilter = selectedStatus; 
                FilterVolunteersByStatus(selectedStatus);
            }
        }

        private void TxtVolunteerNameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            _currentNameFilter = (sender as TextBox)?.Text ?? string.Empty; 
            FilterVolunteersByName(_currentNameFilter);
        }

        private void FilterVolunteersByName(string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                VolunteerList = _allVolunteers; 
            }
            else
            {
                VolunteerList = _allVolunteers
                                .Where(volunteer => volunteer.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                                .ToList();
            }
        }

        private void FilterVolunteersByStatus(BO.CallType? callType)
        {
            if (callType == null || callType == BO.CallType.None)
            {
                VolunteerList = _allVolunteers; 
            }
            else
            {
                VolunteerList = _allVolunteers
                                .Where(volunteer => volunteer.CurrentCallType == callType)
                                .ToList();
            }
        }

        private void CheckBox_IsActiveChanged(object sender, RoutedEventArgs e)
        {
            // Ensure the window is fully loaded before processing the event
            if (!_isWindowLoaded)
                return;

            // Check if the event is triggered by user interaction
            if (sender is CheckBox checkBox && checkBox.IsFocused) // IsFocused ensures it's user interaction
            {
                if (checkBox.DataContext is BO.VolunteerInList volunteerInList)
                {
                    try
                    {
                        {
                            // Retrieve the full Volunteer object using the ID
                            BO.Volunteer volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerInList.Id);

                            // Update the IsActive property
                            volunteer.IsActive = checkBox.IsChecked ?? false;

                            // Send the updated volunteer back to the backend
                            s_bl.Volunteer.UpdateVolunteerAsync(volunteer.Id, volunteer);

                            MessageBox.Show($"Volunteer '{volunteer.Name}' active status updated to '{volunteer.IsActive}'.",
                                            "Status Updated", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Refresh the volunteer list
                            _allVolunteers = s_bl.Volunteer.GetVolunteersList();
                            VolunteerList = _allVolunteers;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to update active status for volunteer '{volunteerInList.Name}': {ex.Message}",
                                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                        // Revert the checkbox state to its original value
                        checkBox.IsChecked = volunteerInList.IsActive;
                    }
                }
            }
        }
    }

    public class CallTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BO.CallType callType)
            {
                return callType switch
                {
                    BO.CallType.None => Brushes.Gray,
                    BO.CallType.PrepareFood => Brushes.Orange,
                    BO.CallType.CleanClothes => Brushes.LightBlue,
                    BO.CallType.DeliverFood => Brushes.Green,
                    BO.CallType.ProvideTransportation => Brushes.Purple,
                    BO.CallType.OrganizeEvents => Brushes.Gold,
                    BO.CallType.SupplyDistribution => Brushes.DarkGreen,
                    BO.CallType.MedicalSupport => Brushes.Red,
                    BO.CallType.PsychologicalSupport => Brushes.Pink,
                    BO.CallType.EquipmentRepair => Brushes.Brown,
                    BO.CallType.CampSetup => Brushes.Teal,
                    BO.CallType.CleanFacilities => Brushes.LightGray,
                    BO.CallType.ITSupport => Brushes.Cyan,
                    BO.CallType.LanguageTranslation => Brushes.LightGreen,
                    BO.CallType.Tutoring => Brushes.Yellow,
                    _ => Brushes.Black
                };
            }

            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}