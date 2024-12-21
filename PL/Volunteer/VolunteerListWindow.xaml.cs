using BO;
using PL.Call;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();


        //public IEnumerable<BO.VolunteerInList> VolunteerList;
        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerInListProperty); }
            set { SetValue(VolunteerInListProperty, value); }
        }
        // Using DependencyProperty as the backing store for VolunteerList
        public static readonly DependencyProperty VolunteerInListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public BO.CallType Type { get; set; } = BO.CallType.None;

        public VolunteerListWindow()
        {
            InitializeComponent();
        }



        private void CbVolunteerTypeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryVolunteerList();
        }


        private void queryVolunteerList()
        => VolunteerList = (Type == BO.CallType.None) ?
        s_bl?.Volunteer.GetVolunteersList()! : s_bl?.Volunteer.GetVolunteersList(Type, BO.VolunteerInListSortFields.CallType)!;

        private void volunteerListObserver()
            => queryVolunteerList();
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(volunteerListObserver);
        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(volunteerListObserver);


        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        private void lsvVolunteerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
                new VolunteerWindow(SelectedVolunteer.Id).Show();
        }

        private void AddvolunteerButton_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow().Show();
        }
    }
}
