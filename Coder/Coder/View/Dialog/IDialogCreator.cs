using System.Windows;

namespace Xarivu.Coder.View.Dialog
{
    public interface IDialogCreator
    {
        Window CreateDialog(string name);
    }
}
