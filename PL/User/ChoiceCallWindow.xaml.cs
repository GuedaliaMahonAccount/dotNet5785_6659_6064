using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PL.User
{
    /// <summary>
    /// Interaction logic for ChoiceCallWindow.xaml
    /// </summary>
    public partial class ChoiceCallWindow : Window
    {
        // Static reference to business logic layer
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public ChoiceCallWindow(int id)
        {
            InitializeComponent();
            LoadCalls();
        }

        private void LoadCalls()
        {
            try
            {
                // Fetch calls with specific CallType values
                var calls = s_bl.Call.GetCallList()
                    .Where(call => call.CallType == BO.CallType.Open
                                   || call.CallType == BO.CallType.SelfCanceled
                                   || call.CallType == BO.CallType.AdminCanceled
                                   || call.CallType == BO.CallType.OpenAtRisk)
                    .ToList();

                CallDataGrid.ItemsSource = calls;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
