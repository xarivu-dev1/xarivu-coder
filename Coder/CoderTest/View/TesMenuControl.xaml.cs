using Xarivu.Coder.Utilities;
using Xarivu.CoderTest.ViewModel;
using System.Windows.Controls;

namespace Xarivu.CoderTest.View
{
    /// <summary>
    /// Interaction logic for TesMenuControl.xaml
    /// </summary>
    public partial class TesMenuControl : UserControl
    {
        public TesMenuControl()
        {
            InitializeComponent();

            ViewUtilities.SetDataContext<TestMenuControlViewModel>(this);
        }
    }
}
