using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PL.Call
{
    public partial class AssignmentCall : Window
    {
        public ObservableCollection<BO.CallAssignInList> Assignments { get; set; }

        public AssignmentCall(BO.Call call)
        {
            // Sort assignments by StartTime before loading them into the ObservableCollection
            Assignments = new ObservableCollection<BO.CallAssignInList>(
                (call.Assignments ?? new List<BO.CallAssignInList>())
                .OrderBy(a => a.StartTime));

            InitializeComponent();
            DataContext = this;
        }
    }
}
