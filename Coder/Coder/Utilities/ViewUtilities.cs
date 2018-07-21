using System.ComponentModel;
using System.Windows;
using Xarivu.Coder.Model.Tracked;

namespace Xarivu.Coder.Utilities
{
    public static class ViewUtilities
    {
        public static T SetDataContext<T>(FrameworkElement uc) where T : NotifyChangeBase
        {
            if (!DesignerProperties.GetIsInDesignMode(uc))
            {
                var dc = DependencyContainer.Get<T>();
                uc.DataContext = dc;
                return dc;
            }

            return null;
        }
    }
}
