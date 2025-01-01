using System.Windows;


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
    }


}
