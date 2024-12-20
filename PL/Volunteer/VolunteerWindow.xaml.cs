using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // Dependency property for button text
        string ButtonText
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
            DataContext = this;
        }

        // Event handler for Add/Update button
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get password from PasswordBox
                var passwordBox = this.FindName("PasswordBox") as PasswordBox;
                if (passwordBox != null)
                {
                    CurrentVolunteer.Password = passwordBox.Password;
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(CurrentVolunteer.Name) ||
                    string.IsNullOrWhiteSpace(CurrentVolunteer.Phone) ||
                    string.IsNullOrWhiteSpace(CurrentVolunteer.Email))
                {
                    MessageBox.Show("Please fill in all required fields.", "Validation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer); // Add new Volunteer
                    MessageBox.Show("Volunteer added successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer); // Update existing Volunteer
                    MessageBox.Show("Volunteer updated successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}