using PL.Call;
using PL.Volunteer;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

        public int MaxYearRange
        {
            get { return (int)GetValue(MaxYearRangeProperty); }
            set { SetValue(MaxYearRangeProperty, value); }
        }

        public static readonly DependencyProperty MaxYearRangeProperty =
            DependencyProperty.Register("MaxYearRange", typeof(int), typeof(MainWindow), new PropertyMetadata(0));

        public MainWindow()
        {
            InitializeComponent();
        }

        private void clockObserver()
        {
            CurrentTime = s_bl.Admin.GetCurrentTime();
        }

        private void configObserver()
        {
            TimeSpan riskTime = s_bl.Admin.GetRiskTime();
            int years = (int)(riskTime.TotalDays / 365);
            MaxYearRange = years;
        }

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.UpdateClock(BO.TimeUnit.MINUTE);
        }

        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.UpdateClock(BO.TimeUnit.HOUR);
        }

        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.UpdateClock(BO.TimeUnit.DAY);
        }

        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.UpdateClock(BO.TimeUnit.MONTH);
        }

        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.UpdateClock(BO.TimeUnit.YEAR);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetCurrentTime();

            TimeSpan riskTime = s_bl.Admin.GetRiskTime();
            MaxYearRange = (int)(riskTime.TotalDays / 365);

            s_bl.Admin.AddClockObserver(clockObserver);

            s_bl.Admin.AddConfigObserver(configObserver);
        }

        private void Window_Closed(object sender, EventArgs e) 
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);

            foreach (Window window in Application.Current.Windows)
            {
                if (window != this) 
                {
                    window.Close();
                }
            }
        }

        private void btnUpdateMaxYearRange_Click(object sender, RoutedEventArgs e)
        {
            int maxYearRange = MaxYearRange;

            TimeSpan riskTimeSpan = TimeSpan.FromDays(maxYearRange * 365);

            s_bl.Admin.SetRiskTime(riskTimeSpan);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
       
        private void btnHandleVolunteer_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerListWindow().Show();
        }

        private void btnHandleCall_Click(object sender, RoutedEventArgs e)
        {
            new CallListWindow().Show();
        }


        private void btnResetDB_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to reset the database?", "Reset Database", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this)
                        window.Close();
                }

                s_bl.Admin.ResetDatabase();
                MessageBox.Show("Database reset successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to reset the database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void btnInitializeDB_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to initialize the database?", "Initialize Database", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this)
                        window.Close();
                }
                s_bl.Admin.InitializeDatabase();

                MessageBox.Show("Database initialized successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize the database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

    }
}
