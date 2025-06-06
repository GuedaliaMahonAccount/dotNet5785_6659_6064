﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using BO;

namespace PL
{
    public partial class AdminMenu : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private DispatcherTimer _timer = new DispatcherTimer();

        // Dependency Properties


        private DateTime _lastSimulatorTime; // Last time shown by the simulator
        private DateTime _lastRealTime;     // Real time when the simulator stopped
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

        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(AdminMenu), new PropertyMetadata(1));

        public bool IsSimulatorRunning
        {
            get { return (bool)GetValue(IsSimulatorRunningProperty); }
            set { SetValue(IsSimulatorRunningProperty, value); }
        }
        public static readonly DependencyProperty IsSimulatorRunningProperty =
            DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(AdminMenu), new PropertyMetadata(false));

        public string SimulatorButtonText
        {
            get { return (string)GetValue(SimulatorButtonTextProperty); }
            set { SetValue(SimulatorButtonTextProperty, value); }
        }
        public static readonly DependencyProperty SimulatorButtonTextProperty =
            DependencyProperty.Register("SimulatorButtonText", typeof(string), typeof(AdminMenu), new PropertyMetadata("Start Simulator"));

        public ObservableCollection<KeyValuePair<string, int>> CallQuantities { get; set; } = new ObservableCollection<KeyValuePair<string, int>>();
        public ICommand NavigateToCallListCommand { get; private set; }

        // Flags to prevent multiple updates
        private volatile bool _isUpdatingCallQuantities = false;
        private volatile bool _isUpdatingClockObserver = false;
        private volatile bool _isUpdatingConfigObserver = false;

        public AdminMenu()
        {
            InitializeComponent();
            CallQuantities = new ObservableCollection<KeyValuePair<string, int>>();
            //InitializeTimer();
            NavigateToCallListCommand = new RelayCommand<string>(NavigateToCallList);
            DataContext = this;  // Set this after initializing all properties
            LoadCallQuantities();  // Load the data after setting DataContext
        }

        private void btnToggleSimulator_Click(object sender, RoutedEventArgs e)
        {
            if (IsSimulatorRunning)
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
                SimulatorButtonText = "Start Simulator";

                // Save the last simulator time and the real time when stopping
                _lastSimulatorTime = s_bl.Admin.GetCurrentTime();
                _lastRealTime = DateTime.Now;
            }
            else
            {
                s_bl.Admin.StartSimulator(Interval / 60);
                IsSimulatorRunning = true;
                SimulatorButtonText = "Stop Simulator";
            }
        }

        private void LoadCallQuantities()
        {
            if (!_isUpdatingCallQuantities)
            {
                _isUpdatingCallQuantities = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        int[] quantities = s_bl.Call.GetCallQuantities();
                        CallQuantities.Clear();
                        foreach (BO.Status status in Enum.GetValues(typeof(BO.Status)))
                        {
                            CallQuantities.Add(new KeyValuePair<string, int>(
                                status.ToString(),
                                quantities[(int)status]));
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading call quantities: {ex.Message}", "Error",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        _isUpdatingCallQuantities = false; // Reset the flag after the operation is done
                    }
                }));
            }
        }

        private void NavigateToCallList(string callTypeStr)
        {
            if (Enum.TryParse(callTypeStr, out BO.Status status))
            {
                new Call.CallListWindow(status).Show();
            }
        }

        private void InitializeTimer()
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) =>
            {
                if (IsSimulatorRunning)
                {
                    // Update the time using the simulator
                    _lastSimulatorTime = s_bl.Admin.GetCurrentTime();
                    CurrentTime = _lastSimulatorTime;
                }
                else
                {
                    // Continue from the last simulator time, adding the real-time difference
                    TimeSpan timeSinceStop = DateTime.Now - _lastRealTime;
                    CurrentTime = _lastSimulatorTime + timeSinceStop;
                }
            };
            _timer.Start();
        }

        private void clockObserver()
        {
            if (!_isUpdatingClockObserver)
            {
                _isUpdatingClockObserver = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        CurrentTime = s_bl.Admin.GetCurrentTime();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in clockObserver: {ex.Message}");
                    }
                    finally
                    {
                        _isUpdatingClockObserver = false; // Reset the flag after the operation is done
                    }
                }));
            }
        }

        private void configObserver()
        {
            if (!_isUpdatingConfigObserver)
            {
                _isUpdatingConfigObserver = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        TimeSpan riskTime = s_bl.Admin.GetRiskTime();
                        MaxYearRange = (int)(riskTime.TotalDays);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in configObserver: {ex.Message}");
                    }
                    finally
                    {
                        _isUpdatingConfigObserver = false; // Reset the flag after the operation is done
                    }
                }));
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
            new Call.CallListWindow(BO.Status.None).Show();
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
            if (IsSimulatorRunning)
            {
                s_bl.Admin.StopSimulator();
            }
            if (!Login.isMainWindowClosing)
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

    public class SimulatorRunningToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isSimulatorRunning = (bool)value;
            bool invert = parameter as string == "Invert";
            return invert ? !isSimulatorRunning : isSimulatorRunning;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}