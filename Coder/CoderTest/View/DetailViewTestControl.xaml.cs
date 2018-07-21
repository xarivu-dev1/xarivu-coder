using Xarivu.Coder.Utilities;
using Xarivu.CoderTest.ViewModel;
using System.Windows.Controls;

namespace Xarivu.CoderTest.View
{
    /// <summary>
    /// Interaction logic for DetailViewControl.xaml
    /// </summary>
    public partial class DetailViewTestControl : UserControl
    {
        public DetailViewTestControl()
        {
            InitializeComponent();

            ViewUtilities.SetDataContext<DetailViewTestControlViewModel>(this);
        }
    }
}
