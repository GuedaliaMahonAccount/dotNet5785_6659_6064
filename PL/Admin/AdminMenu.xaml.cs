using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL
{
    /// <summary>
    /// Interaction logic for AdminMenu.xaml
    /// </summary>

    public partial class AdminMenu : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(AdminMenu));

        public int MaxYearRange
        {
            get { return (int)GetValue(MaxYearRangeProperty); }
            set { SetValue(MaxYearRangeProperty, value); }
        }

        public static readonly DependencyProperty MaxYearRangeProperty =
            DependencyProperty.Register("MaxYearRange", typeof(int), typeof(AdminMenu), new PropertyMetadata(0));

        public AdminMenu()
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

            //foreach (Window window in Application.Current.Windows)
            //{
            //    if (window != this)
            //    {
            //        window.Close();
            //    }
            //}
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

        private void BtnHandleVolunteer_Click(object sender, RoutedEventArgs e)
        {
            new Volunteer.VolunteerListWindow().Show();
        }

        private void BtnHandleCall_Click(object sender, RoutedEventArgs e)
        {
            new Call.CallListWindow().Show();
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
