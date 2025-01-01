using BO;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary> 
    public partial class CallListWindow : Window
    {
        // Static reference to business logic layer
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // Property for list of calls displayed in UI
        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallInListProperty); }
            set { SetValue(CallInListProperty, value); }
        }

        // Dependency property for CallList
        public static readonly DependencyProperty CallInListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>),
            typeof(CallListWindow), new PropertyMetadata(null));

        // Property for filtering calls by type
        public BO.CallType Type { get; set; } = BO.CallType.None;

        // Property for tracking selected call in list
        public BO.CallInList? SelectedCall { get; set; }

        /// <summary>
        /// Constructor - initializes window and loads initial call list
        /// </summary>
        public CallListWindow()
        {
            InitializeComponent();
            queryCallList();
        }

        /// <summary>
        /// Handler for call type filter selection change
        /// </summary>
        private void CbCallTypeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryCallList();
        }

        /// <summary>
        /// Queries and updates the list of calls based on current filter
        /// </summary>
        private void queryCallList()
            => CallList = (Type == BO.CallType.None) ?
                           s_bl?.Call.GetCallList()! : s_bl?.Call.GetCallList(Type, BO.CallSortField.CallType)!;

        /// <summary>
        /// Observer callback - refreshes call list when data changes
        /// </summary>
        private void callListObserver()
            => queryCallList();

        /// <summary>
        /// Window load handler - sets up observer and loads initial data
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(callListObserver);

        /// <summary>
        /// Window close handler - removes observer
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(callListObserver);
        }

        /// <summary>
        /// Double click handler for call list items - opens call details window
        /// </summary>
        private void lsvCallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
            {
                if (SelectedCall.CallId.HasValue)
                {
                    new CallWindow(SelectedCall.CallId.Value).Show();
                }
                else
                {
                    MessageBox.Show("Selected call does not have a valid ID.");
                }
            }
        }

        /// <summary>
        /// Handler for add call button click - opens new call window
        /// </summary>
        private void AddCallButton_Click(object sender, RoutedEventArgs e)
        {
            new CallWindow().Show();
        }
    }

    /// <summary>
    /// Converts TimeSpan values to human readable strings
    /// </summary>
    public class TimeSpanToLabeledStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                return $"{timeSpan.Days} days, {timeSpan.Hours} hours, {timeSpan.Minutes} minutes, {timeSpan.Seconds} seconds";
            }
            return ""; // Return empty string for null or invalid values
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}