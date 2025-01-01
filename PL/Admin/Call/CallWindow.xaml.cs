using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // depency properties for button text
        string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            init => SetValue(ButtonTextProperty, value);
        }
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(CallWindow));


        // Dependency property for CurrentCall
        public BO.Call CurrentCall
        {
            get => (BO.Call)GetValue(CurrentCallProperty);
            set => SetValue(CurrentCallProperty, value);
        }
        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register(nameof(CurrentCall), typeof(BO.Call), typeof(CallWindow));


        public CallWindow(int id = 0)
        {
            ButtonText = id == 0 ? "Add" : "Update";


            // Initialize CurrentCall based on whether an id is provided
            CurrentCall = id == 0
                ? new BO.Call() // New Call
                : s_bl.Call.GetCallDetails(id); // Fetch existing Call from BL

            InitializeComponent();
        }


        // Event handler for Add/Update button
        private void BtnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    s_bl.Call.AddCall(CurrentCall); // Add new Call
                    MessageBox.Show("Call added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    s_bl.Call.UpdateCall(CurrentCall); // Update existing Call
                    MessageBox.Show("Call updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }

    public class UpdateToReadOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !int.TryParse(value.ToString(), out int intValue) || intValue == 0)
            {
                return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class UpdateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !int.TryParse(value.ToString(), out int intValue) || intValue == 0)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

}