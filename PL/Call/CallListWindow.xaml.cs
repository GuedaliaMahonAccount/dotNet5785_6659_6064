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
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();


        //public IEnumerable<BO.CallInList> CallList;
        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallInListProperty); }
            set { SetValue(CallInListProperty, value); }
        }
        // Using DependencyProperty as the backing store for CallList
        public static readonly DependencyProperty CallInListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

        public BO.CallType Type { get; set; } = BO.CallType.None;

        public CallListWindow()
        {
            InitializeComponent();
        }

        private void CbCallTypeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryCallList();
        }

        private void queryCallList()
    => CallList = (Type == BO.CallType.None) ?
        s_bl?.Call.GetCallList()! : s_bl?.Call.GetCallList(Type, BO.CallSortField.CallType)!;

        private void callListObserver()
            => queryCallList();
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(callListObserver);
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(callListObserver);


        public BO.CallInList? SelectedCall { get; set; }

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
                return $"{timeSpan.Days} days, {timeSpan.Hours} hours, {timeSpan.Minutes} minutes, {timeSpan.Seconds} seconds";
            }

            return ""; // If the value is null or not a TimeSpan
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
