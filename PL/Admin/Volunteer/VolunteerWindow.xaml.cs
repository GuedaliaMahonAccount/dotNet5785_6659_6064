using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window, INotifyPropertyChanged
    {
        // Property to toggle password visibility
        private bool _isPasswordVisible;
        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged();
            }
        }

        public ICommand TogglePasswordVisibilityCommand { get; }

        // Static reference to business logic layer
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // Dependency property for button text
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            init => SetValue(ButtonTextProperty, value);
        }
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow));

        // Dependency property for CurrentVolunteer
        public BO.Volunteer CurrentVolunteer
        {
            get => (BO.Volunteer)GetValue(CurrentVolunteerProperty);
            set => SetValue(CurrentVolunteerProperty, value);
        }
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register(nameof(CurrentVolunteer), typeof(BO.Volunteer), typeof(VolunteerWindow));

        public VolunteerWindow(int id = 0)
        {
            TogglePasswordVisibilityCommand = new RelayCommand(() => IsPasswordVisible = !IsPasswordVisible);

            ButtonText = id == 0 ? "Add" : "Update";

            // Initialize CurrentVolunteer based on whether an ID is provided
            CurrentVolunteer = id == 0
                ? new BO.Volunteer() // New Volunteer
                : s_bl.Volunteer.GetVolunteerDetails(id); // Fetch existing Volunteer from BL

            InitializeComponent();

            // Attach event handlers for loading and closing
            Loaded += VolunteerWindow_Loaded;
            Closed += VolunteerWindow_Closed;
        }

        private void VolunteerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(UpdateVolunteerData);
        }
        private void VolunteerWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(UpdateVolunteerData);
        }
        private void UpdateVolunteerData()
        {
            if (CurrentVolunteer?.Id != null)
            {
                var updatedVolunteer = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id);
                if (updatedVolunteer != null)
                {
                    CurrentVolunteer = updatedVolunteer;
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Event handler for Add/Update button
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer); // Add new Volunteer
                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer); // Update existing Volunteer
                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // see current call window
        private void btnViewCurrentCall_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer.CurrentCall == null)
            {
                MessageBox.Show("No current call assigned.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var currentCallWindow = new CurrentCallWindow(CurrentVolunteer.CurrentCall);
            currentCallWindow.ShowDialog();
        }

    }


    /// <summary>
    /// helpers
    /// </summary>
    /// 
    ///
    // Helper class for PasswordBox binding
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static string GetBoundPassword(DependencyObject obj) =>
            (string)obj.GetValue(BoundPassword);

        public static void SetBoundPassword(DependencyObject obj, string value) =>
            obj.SetValue(BoundPassword, value);

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox && !passwordBox.Password.Equals(e.NewValue))
            {
                passwordBox.Password = e.NewValue?.ToString();
            }
        }

        public static readonly DependencyProperty BindPassword =
            DependencyProperty.RegisterAttached("BindPassword", typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false, OnBindPasswordChanged));

        public static bool GetBindPassword(DependencyObject obj) =>
            (bool)obj.GetValue(BindPassword);

        public static void SetBindPassword(DependencyObject obj, bool value) =>
            obj.SetValue(BindPassword, value);

        private static void OnBindPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
                }
                else
                {
                    passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                }
            }
        }

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetBoundPassword(passwordBox, passwordBox.Password);
            }
        }
    }

    // Converter for IsReadOnly based on Update state
    public class UpdateToReadOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is string buttonText && buttonText == "Update";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    // Converter for Visibility based on Update state
    public class UpdateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null || (int)value == 0 ? Visibility.Visible : Visibility.Collapsed; // Show for Add, Hide for Update
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    // Converter for Visibility based on Update state
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is bool isVisible && isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter for Visibility based on Update state
    public class InvertedBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is bool isVisible && !isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Converter for Eye icon based on visibility
    public class BoolToEyeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is bool isVisible && isVisible ? "🙈" : "👁";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    // RelayCommand Implementation
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;


        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
