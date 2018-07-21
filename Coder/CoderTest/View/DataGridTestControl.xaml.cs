using Xarivu.Coder.Utilities;
using Xarivu.CoderTest.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Xarivu.CoderTest.View
{
    /// <summary>
    /// Interaction logic for DataGridTestControl.xaml
    /// </summary>
    public partial class DataGridTestControl : UserControl
    {
        public DataGridTestControl()
        {
            InitializeComponent();

            ViewUtilities.SetDataContext<DataGridTestControlViewModel>(this);
        }

        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid != null)
            {
                dataGrid.SelectedItem = null;
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid != null)
            {
                var item = dataGrid.SelectedItem;
                if (item?.ToString() == "{NewItemPlaceholder}")
                {
                    //dataGrid.SelectedItem = null;
                }
            }
        }
    }
}
