﻿using System.IO;
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
            s_bl.Admin.InitializeDB();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }



        internal static bool isMainWindowClosing = false;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender == Application.Current.MainWindow)
            {
                var result = MessageBox.Show("Are you sure you want to close this window?",
                                              "Confirm Close",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    isMainWindowClosing = true; 
                    Application.Current.Shutdown();
                }
            }
            else
            {
                if (isMainWindowClosing)
                {
                    e.Cancel = false;
                }
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
