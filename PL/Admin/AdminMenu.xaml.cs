using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using BO;

namespace PL
{
    public partial class AdminMenu : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private DispatcherTimer _timer = new DispatcherTimer();

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

        public ObservableCollection<KeyValuePair<string, int>> CallQuantities { get; set; } = new ObservableCollection<KeyValuePair<string, int>>();
        public ICommand NavigateToCallListCommand { get; private set; }

        public AdminMenu()
        {
            InitializeComponent();
            CallQuantities = new ObservableCollection<KeyValuePair<string, int>>();
            InitializeTimer();
            NavigateToCallListCommand = new RelayCommand<string>(NavigateToCallList);
            DataContext = this;  // Set this after initializing all properties
            LoadCallQuantities();  // Load the data after setting DataContext
        }

        private void LoadCallQuantities()
        {
            try
            {
                int[] quantities = s_bl.Call.GetCallQuantities();
                CallQuantities.Clear();
                foreach (BO.CallType callType in Enum.GetValues(typeof(BO.CallType)))
                {
                    //if (callType != BO.CallType.None)
                    {
                        CallQuantities.Add(new KeyValuePair<string, int>(
                            callType.ToString(),
                            quantities[(int)callType]));
                    }
                }

                // Debug: Vérifiez que la collection est remplie
                Debug.WriteLine($"CallQuantities count: {CallQuantities.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading call quantities: {ex.Message}", "Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NavigateToCallList(string callTypeStr)
        {
            if (Enum.TryParse(callTypeStr, out BO.CallType callType))
            {
                new Call.CallListWindow(callType).Show();
            }
        }

        private void InitializeTimer()
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) => CurrentTime = DateTime.Now;
            _timer.Start();
        }

        private void clockObserver()
        {
            try
            {
                CurrentTime = s_bl.Admin.GetCurrentTime();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in clockObserver: {ex.Message}");
            }
        }

        private void configObserver()
        {
            try
            {
                TimeSpan riskTime = s_bl.Admin.GetRiskTime();
                MaxYearRange = (int)(riskTime.TotalDays );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in configObserver: {ex.Message}");
            }
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
            try
            {
                CurrentTime = s_bl.Admin.GetCurrentTime();
                TimeSpan riskTime = s_bl.Admin.GetRiskTime();
                MaxYearRange = (int)(riskTime.TotalDays);

                s_bl.Admin.AddClockObserver(clockObserver);
                s_bl.Admin.AddConfigObserver(configObserver);
                s_bl.Call.AddObserver(LoadCallQuantities);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during Window_Loaded: {ex.Message}");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            s_bl.Call.RemoveObserver(LoadCallQuantities);
            _timer?.Stop();
        }

        private void btnUpdateMaxYearRange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TimeSpan riskTimeSpan = TimeSpan.FromDays(MaxYearRange);
                s_bl.Admin.SetRiskTime(riskTimeSpan);
                MessageBox.Show("Risk time updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update risk time: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnHandleVolunteer_Click(object sender, RoutedEventArgs e)
        {
            new Volunteer.VolunteerListWindow().Show();
        }

        private void BtnHandleCall_Click(object sender, RoutedEventArgs e)
        {
            new Call.CallListWindow(BO.CallType.None).Show();
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

                s_bl.Admin.InitializeDB();
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

                s_bl.Admin.InitializeDB();
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

        public class RelayCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            private readonly Predicate<T> _canExecute;

            public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;

            public void Execute(object parameter) => _execute((T)parameter);

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }
        }
    }
}