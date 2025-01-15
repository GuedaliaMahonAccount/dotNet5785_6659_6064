using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            if (textBox != null && textBox.Text == "Enter Id")
            {
                textBox.Text = string.Empty;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrEmpty(textBox.Text))
            {
                if (textBox.Name == "IdTextBox")
                {
                    textBox.Text = "Enter Id";
                }
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null && passwordBox.Password == "Enter Password")
            {
                passwordBox.Password = string.Empty;
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null && string.IsNullOrEmpty(passwordBox.Password))
            {
                passwordBox.Password = "Enter Password";
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string volunteerId = IdTextBox.Text;
            string encryptedPassword = EncryptionHelper.Encrypt(PasswordTextBox.Password);

            int AdminId = 322766064;

            try
            {
                BO.Volunteer adminVolunteer = s_bl.Volunteer.GetVolunteerDetails(AdminId);

                if (adminVolunteer != null && adminVolunteer.Id == int.Parse(volunteerId) && adminVolunteer.Password == encryptedPassword)
                {
                    AdminMenu adminMenu = new AdminMenu();
                    adminMenu.Show();
                }
                else
                {
                    BO.Volunteer GeneralVolunteer = s_bl.Volunteer.GetVolunteerDetails(int.Parse(volunteerId));

                    if (GeneralVolunteer != null && GeneralVolunteer.Id == int.Parse(volunteerId) && GeneralVolunteer.Password == encryptedPassword)
                    {
                        MainUserWindow volunteerMenu = new MainUserWindow(int.Parse(volunteerId));
                        volunteerMenu.Show();
                    }
                    else
                    {
                        MessageBox.Show("Invalid Id or password", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class EncryptionHelper
        {
            private static readonly string Key = "YourSecureKey123";
            private static readonly string IV = "YourSecureIV1234";

            public static string Encrypt(string plainText)
            {
                using Aes aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = Encoding.UTF8.GetBytes(IV);

                using MemoryStream ms = new();
                using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
                using (StreamWriter writer = new(cs))
                {
                    writer.Write(plainText);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
