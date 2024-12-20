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

        // Dependency property for CurrentStudent
        public BO.Student CurrentStudent
        {
            get => (BO.Student)GetValue(CurrentStudentProperty);
            set => SetValue(CurrentStudentProperty, value);
        }
        public static readonly DependencyProperty CurrentStudentProperty =
            DependencyProperty.Register(nameof(CurrentStudent), typeof(BO.Student), typeof(VolunteerWindow));

        // Collections for ComboBoxes
        public IEnumerable<string> RolesCollection { get; init; }
        public IEnumerable<string> DistanceTypesCollection { get; init; }
        public IEnumerable<BO.Call> CallsCollection { get; init; }

        public VolunteerWindow(int id = 0)
        {
            ButtonText = id == 0 ? "Add" : "Update";

            // Initialize collections for ComboBoxes
            RolesCollection = s_bl.Student.GetRoles();
            DistanceTypesCollection = s_bl.Student.GetDistanceTypes();
            CallsCollection = s_bl.Call.GetAllCalls();

            // Initialize CurrentStudent based on whether an id is provided
            CurrentStudent = id == 0
                ? new BO.Student() // New Student
                : s_bl.Student.GetStudentDetails(id); // Fetch existing Student from BL

            InitializeComponent();
            DataContext = this;
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get password from PasswordBox
                var passwordBox = this.FindName("passwordBox") as PasswordBox;
                if (passwordBox != null)
                {
                    CurrentStudent.Password = passwordBox.Password;
                }

                if (ButtonText == "Add")
                {
                    s_bl.Student.AddStudent(CurrentStudent); // Add new Student
                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    s_bl.Student.UpdateStudent(CurrentStudent); // Update existing Student
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
}