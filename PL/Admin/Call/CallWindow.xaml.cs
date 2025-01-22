using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

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

        private volatile DispatcherOperation _updateCallDataOperation = null;

        public CallWindow(int id = 0)
        {
            ButtonText = id == 0 ? "Add" : "Update";

            CurrentCall = id == 0
                ? new BO.Call()
                : s_bl.Call.GetCallDetails(id);

            InitializeComponent();

            Loaded += CallWindow_Loaded;
            Closed += CallWindow_Closed;
        }

        private void CallWindow_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(UpdateCallData);
        }

        private void CallWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(UpdateCallData);
        }

        private void UpdateCallData()
        {
            if (_updateCallDataOperation == null || _updateCallDataOperation.Status == DispatcherOperationStatus.Completed)
            {
                _updateCallDataOperation = Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (CurrentCall?.Id != null)
                    {
                        var updatedCall = s_bl.Call.GetCallDetails(CurrentCall.Id);
                        if (updatedCall != null)
                        {
                            CurrentCall = updatedCall;
                        }
                    }
                }));
            }
        }

        // Event handler for Add/Update button
        private void BtnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    s_bl.Call.AddCallAsync(CurrentCall); // Add new Call
                    MessageBox.Show("Call added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    s_bl.Call.UpdateCallAsync(CurrentCall); // Update existing Call
                    MessageBox.Show("Call updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // see all the assignement for this call
        private void BtnViewAssignments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open the AssignmentCall window and pass the CurrentCall object
                var assignmentWindow = new AssignmentCall(CurrentCall);
                assignmentWindow.ShowDialog(); // Open as a modal window
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
            if (value is string buttonText && buttonText == "Update")
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