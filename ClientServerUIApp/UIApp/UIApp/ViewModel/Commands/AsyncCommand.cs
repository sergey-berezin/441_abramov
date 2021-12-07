using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UIApp.ViewModel.Commands
{
    public class AsyncCommand : IAsyncCommand
    {
        #region Fields

        private Action<object> targetExecuteMethod = null;

        private Func<object, bool> targetCanExecuteMethod = null;

        private bool isCanceled, isExecution, canExecute;

        #endregion

        #region Constructors

        public AsyncCommand(Action<object> executeMethod)
        { 
            targetExecuteMethod = executeMethod;
            IsExecution = IsCanceled = false;
        }

        public AsyncCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod) :
            this(executeMethod) => targetCanExecuteMethod = canExecuteMethod;

        #endregion

        #region Properties

        public bool IsCanceled
        {
            get => isCanceled;
            set
            {
                isCanceled = value;
                RaisePropertyChanged(nameof(IsCanceled));
                RaiseCanExecuteChanged();
            }
        }

        public bool IsExecution
        {
            get => isExecution;
            set
            {
                isExecution = value;
                RaisePropertyChanged(nameof(IsExecution));
                RaiseCanExecuteChanged();
            }
        }

        public bool CanExecute
        {
            get => canExecute;
            set
            {
                if (canExecute != value)
                {
                    canExecute = value;
                    RaiseCanExecuteChanged();
                }
            }
        }
        
        #endregion

        #region Events

        public event EventHandler CanExecuteChanged;
        
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Events Raisers

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region IAsyncCommand realization methods

        public async void Execute(object parameter)
        {
            IsExecution = true;
            await ExecuteAsync(parameter);
            IsExecution = false;
            IsCanceled = false;
        }

        public Task ExecuteAsync(object parameter) =>
            Task.Factory.StartNew(() => { targetExecuteMethod?.Invoke(parameter); });

        bool ICommand.CanExecute(object parameter)
        {
            CanExecute = targetCanExecuteMethod?.Invoke(parameter) ?? targetExecuteMethod != null;                        
            return CanExecute && !IsExecution;
        }

        #endregion

        public void Cancel()
        {
            IsCanceled = true;
            IsExecution = false;
        }

    }
}
