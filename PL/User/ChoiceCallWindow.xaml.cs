using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;
using BO;

namespace PL.User
{
    public partial class ChoiceCallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public RelayCommand AssignCallCommand { get; }

        public IEnumerable<BO.Call> CallList
        {
            get { return (IEnumerable<BO.Call>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        public RelayCommand ShowDescriptionCommand { get; }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.Call>),
            typeof(ChoiceCallWindow), new PropertyMetadata(null));

        private readonly int _volunteerId;

        // Flag to prevent multiple updates
        private volatile bool _isUpdating = false;

        // DispatcherTimer for periodic updates
        private DispatcherTimer _refreshTimer;

        private bool CanAssignCall()
        {
            try
            {
                var activeCalls = s_bl.Volunteer.GetCurrentCallsForVolunteer(_volunteerId);
                // Close the window after successful assignment
                this.Close();
                return !activeCalls.Any();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to check active calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public ChoiceCallWindow(int volunteerId)
        {
            InitializeComponent();
            _volunteerId = volunteerId;

            DataContext = this;

            ShowDescriptionCommand = new RelayCommand(param =>
            {
                if (param is BO.Call call)
                {
                    MessageBox.Show(call.Description, "Description", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });

            AssignCallCommand = new RelayCommand(param =>
            {
                if (param != null && CanAssignCall())
                {
                    AssignCall((int)param);
                }
                else
                {
                    MessageBox.Show("You already have an active call and cannot assign a new one.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            });

            // Initialize and start the timer
            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(1); // Update every second
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();

            queryCallList();
            s_bl.Call.AddObserver(CallListObserver);
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            queryCallList(); // Refresh the call list periodically
        }

        private void queryCallList()
        {
            if (!_isUpdating)
            {
                _isUpdating = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        // Fetch initial list of calls
                        var callIds = s_bl.Call.GetCallList()
                            .Where(call => call.CallType == BO.CallType.Open
                                           || call.CallType == BO.CallType.OpenAtRisk
                                           || call.CallType == BO.CallType.AdminCanceled
                                           || call.CallType == BO.CallType.SelfCanceled)
                            .Select(call => call.CallId)
                            .ToList();

                        // Fetch full details for each call and calculate distance
                        CallList = callIds
                            .Select(callId =>
                            {
                                var call = s_bl.Call.GetCallDetails(callId.Value);
                                double distance = s_bl.Call._CalculateDistance(callId.Value, _volunteerId);
                                return call;
                            })
                            .ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to load calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        _isUpdating = false; // Reset the flag after the operation is done
                    }
                }));
            }
        }

        private void CallListObserver()
        {
            if (!_isUpdating)
            {
                _isUpdating = true;
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    queryCallList(); // Refresh the call list on UI thread
                    _isUpdating = false; // Reset the flag after the operation is done
                }));
            }
        }

        private void AssignCall(int callId)
        {
            try
            {
                s_bl.Call.selectionCall(_volunteerId, callId);
                MessageBox.Show($"Call {callId} successfully assigned to volunteer {_volunteerId}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to assign call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            s_bl.Call.RemoveObserver(CallListObserver);
            _refreshTimer.Stop(); // Stop the timer when the window is closed
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}