using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BO;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
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
            ButtonText = id == 0 ? "Add" : "Update";

            // Initialize CurrentVolunteer based on whether an id is provided
            CurrentVolunteer = id == 0
                ? new BO.Volunteer() // New Volunteer
                : s_bl.Volunteer.GetVolunteerDetails(id); // Fetch existing Volunteer from BL

            InitializeComponent();
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

    }

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
}
