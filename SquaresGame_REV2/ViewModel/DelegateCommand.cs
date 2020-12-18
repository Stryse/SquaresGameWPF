using System;
using System.Windows.Input;

namespace SquaresGame_REV2.ViewModel
{
    public class DelegateCommand : ICommand
    {
        #region --Fields--

        private readonly Action<Object> execute;
        private readonly Func<Object, bool> canExecute;

        #endregion

        #region --Events--

        public event EventHandler CanExecuteChanged;

        #endregion


        #region --Constructor--

        public DelegateCommand(Action<Object> execute, Func<Object, bool> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentException("Action is null!", "execute");

            this.execute    = execute;
            this.canExecute = canExecute;
        }

        #endregion

        #region --Public Methods--

        public bool CanExecute(object parameter)
        {
            return (canExecute == null) ? true : canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
                execute(parameter);
        }

        #endregion


        #region --Event Methods--

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
