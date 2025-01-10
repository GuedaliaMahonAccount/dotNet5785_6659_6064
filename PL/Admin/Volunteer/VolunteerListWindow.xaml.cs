using BO;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PL.Volunteer
{
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

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

        public VolunteerListWindow()
        {
            InitializeComponent();
            VolunteerList = s_bl.Volunteer.GetVolunteersList(); // Load all volunteers initially
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(() => VolunteerList = s_bl.Volunteer.GetVolunteersList());
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
            new VolunteerWindow().Show();
        }

        private void DeleteVolunteer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int volunteerId)
            {
                var result = MessageBox.Show("Are you sure you want to delete this volunteer?",
                                             "Confirm Delete",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Volunteer.DeleteVolunteer(volunteerId);
                        // No need to refresh the list manually as the Observer pattern will handle it
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
                FilterVolunteersByStatus(selectedStatus);
            }
        }

        private void FilterVolunteersByStatus(BO.CallType? status)
        {
            if (status == null || status == BO.CallType.None)
            {
                VolunteerList = s_bl.Volunteer.GetVolunteersList();
            }
            else
            {
                VolunteerList = s_bl.Volunteer.GetVolunteersList()
                                              .Where(volunteer => volunteer.CurrentCallType == status)
                                              .ToList();
            }
        }
    }

    /// <summary>
    /// Enum to color converter for CallType.
    /// </summary>
    public class CallTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BO.CallType callType)
            {
                // Map CallType values to colors
                return callType switch
                {
                    BO.CallType.None => Brushes.Gray,
                    BO.CallType.Open => Brushes.Green,
                    BO.CallType.InTreatment => Brushes.Blue,
                    BO.CallType.Completed => Brushes.DarkGreen,
                    BO.CallType.Expired => Brushes.OrangeRed,
                    BO.CallType.SelfCanceled => Brushes.Orange,
                    BO.CallType.AdminCanceled => Brushes.Red,
                    BO.CallType.OpenAtRisk => Brushes.Yellow,
                    BO.CallType.InTreatmentAtRisk => Brushes.Gold,
                    _ => Brushes.Black
                };
            }

            return Brushes.Black; // Default color
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
