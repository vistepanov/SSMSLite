using System;
using System.Windows.Input;
using SsmsLite.Core.Ui.Utils;

namespace SsmsLite.Core.Ui.Commands
{
    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        private readonly ErrorHandler _errorHandler;

        public Command(Action execute, Func<bool> canExecute = null, ErrorHandler errorHandler = null)
        {
            _execute = execute;
            _canExecute = canExecute;
            _errorHandler = errorHandler;
        }

        public bool CanExecute()
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        public void Execute()
        {
            if (CanExecute())
            {
                try
                {
                    _isExecuting = true;
                    _execute();
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            try
            {
                Execute();
            }
            catch (Exception ex)
            {
                _errorHandler?.Invoke(ex);
            }
        }
        #endregion
    }

    public class Command<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private readonly Action<T> _execute;
        private readonly Func<bool> _canExecute;
        private readonly ErrorHandler _errorHandler;

        public Command(Action<T> execute, Func<bool> canExecute = null, ErrorHandler errorHandler = null)
        {
            _execute = execute;
            _canExecute = canExecute;
            _errorHandler = errorHandler;
        }

        public bool CanExecute()
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        public void Execute(T param)
        {
            if (CanExecute())
            {
                try
                {
                    _isExecuting = true;
                    _execute(param);
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            try
            {
                Execute((T)parameter);
            }
            catch (Exception ex)
            {
                _errorHandler?.Invoke(ex);
            }
        }
        #endregion
    }
}
