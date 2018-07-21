using System;
using System.Windows.Input;

namespace Xarivu.Coder.ViewModel
{
    public class DelegateCommand : ICommand
    {
        Action<object> executeDelegate;
        bool canExecute;

        public DelegateCommand(Action<object> executeDelegate, bool canExec = true)
        {
            this.executeDelegate = executeDelegate;
            this.canExecute = canExec;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            this.executeDelegate(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute;
        }

        public void SetCanExecute(bool canExec)
        {
            if (this.canExecute != canExec)
            {
                this.canExecute = canExec;
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
