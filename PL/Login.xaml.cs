using System.Windows;
using System.Windows.Controls;

namespace PL
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public Login()
        {
            InitializeComponent();
            s_bl.Admin.InitializeDatabase();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to close this window?",
                                         "Confirm Close",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && (textBox.Text == "Enter Name" || textBox.Text == "Enter Password"))
            {
                textBox.Text = string.Empty;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrEmpty(textBox.Text))
            {
                if (textBox.Name == "NameTextBox")
                {
                    textBox.Text = "Enter Name";
                }
                else if (textBox.Name == "PasswordTextBox")
                {
                    textBox.Text = "Enter Password";
                }
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string password = PasswordTextBox.Text;

            int AdminId = 322766064;

            try
            {
                BO.Volunteer adminVolunteer = s_bl.Volunteer.GetVolunteerDetails(AdminId);

                if (adminVolunteer != null && adminVolunteer.Name == name && adminVolunteer.Password == password)
                {
                    AdminMenu adminMenu = new AdminMenu();
                    adminMenu.Show();
                }
                else
                {
                    MessageBox.Show("Invalid name or password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}