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

namespace PL.Call
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

    }
}
