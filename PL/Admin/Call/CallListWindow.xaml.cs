    using BO;
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;

    namespace PL.Call
    {
        /// <summary>
        /// Interaction logic for CallListWindow.xaml
        /// </summary>
        public partial class CallListWindow : Window
        {
            // Static reference to business logic layer
            static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

            // Observable collection for binding the list of calls
            public ObservableCollection<BO.CallInList> CallList { get; set; } = new ObservableCollection<BO.CallInList>();

            // Collection for call type filter
            public ObservableCollection<BO.CallType> CallTypeCollection { get; } = new ObservableCollection<BO.CallType>((BO.CallType[])Enum.GetValues(typeof(BO.CallType)));

            // Property for the selected call type filter
            public BO.CallType Type { get; set; }

            // Property for tracking the selected call in the list
            public BO.CallInList? SelectedCall { get; set; }

            /// <summary>
            /// Default constructor - initializes the window with no filter (CallType.None)
            /// </summary>
            public CallListWindow() : this(BO.CallType.None) { }

            /// <summary>
            /// Constructor - initializes the window with a specific call type filter
            /// </summary>
            /// <param name="callType">The call type to filter the list by</param>
            public CallListWindow(BO.CallType callType)
            {
                InitializeComponent();
                Type = callType;
                DataContext = this; // Bind the data context to the current instance
                queryCallList();    // Load the initial call list
            }

            /// <summary>
            /// Event handler for the call type filter ComboBox selection change
            /// </summary>
            private void CbCallTypeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                queryCallList();
            }

            /// <summary>
            /// Queries and updates the list of calls based on the selected call type filter
            /// </summary>
            private void queryCallList()
            {
                CallList.Clear();
                var filteredCalls = Type == BO.CallType.None
                    ? s_bl.Call.GetCallList()
                    : s_bl.Call.GetCallList(Type);

                foreach (var call in filteredCalls)
                {
                    CallList.Add(call);
                }
            }

            /// <summary>
            /// Event handler for the window's Loaded event - adds an observer for refreshing the call list
            /// </summary>
            private void Window_Loaded(object sender, RoutedEventArgs e)
            {
                s_bl.Call.AddObserver(queryCallList);
            }

            /// <summary>
            /// Event handler for the window's Closed event - removes the observer to avoid memory leaks
            /// </summary>
            private void Window_Closed(object sender, EventArgs e)
            {
                s_bl.Call.RemoveObserver(queryCallList);
            }

            /// <summary>
            /// Event handler for double-clicking an item in the call list - opens the CallWindow for the selected call
            /// </summary>
            private void lsvCallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            {
                if (SelectedCall?.CallId != null)
                {
                    new CallWindow(SelectedCall.CallId.Value).Show();
                }
                else
                {
                    MessageBox.Show("Selected call does not have a valid ID.");
                }
            }

            /// <summary>
            /// Event handler for clicking the "Add Call" button - opens a new CallWindow for adding a call
            /// </summary>
            private void AddCallButton_Click(object sender, RoutedEventArgs e)
            {
                new CallWindow().Show();
            }
        }

        public class TimeSpanToLabeledStringConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is TimeSpan timeSpan)
                {
                    var parts = new List<string>();

                    if (timeSpan.Days > 0)
                        parts.Add($"{timeSpan.Days} days");
                    if (timeSpan.Hours > 0)
                        parts.Add($"{timeSpan.Hours} hours");
                    if (timeSpan.Minutes > 0)
                        parts.Add($"{timeSpan.Minutes} minutes");
                    if (timeSpan.Seconds > 0)
                        parts.Add($"{timeSpan.Seconds} seconds");

                    return parts.Count > 0 ? string.Join(", ", parts) : "0";
                }

                return string.Empty; // If value is not a TimeSpan
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public class CallTypeToColorConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is BO.CallType callType)
                {
                    return callType switch
                    {
                        BO.CallType.None => Brushes.Gray,
                        BO.CallType.Open => Brushes.Green,
                        BO.CallType.InTreatment => Brushes.Blue,
                        BO.CallType.Completed => Brushes.DarkGreen,
                        BO.CallType.Expired => Brushes.OrangeRed,
                        BO.CallType.SelfCanceled => Brushes.Orange,
                        BO.CallType.AdminCanceled => Brushes.Red,
                        BO.CallType.OpenAtRisk => Brushes.Yellow,
                        BO.CallType.InTreatmentAtRisk => Brushes.Gold,
                        _ => Brushes.Black
                    };
                }

                return Brushes.Black; // Default color for unknown types
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
