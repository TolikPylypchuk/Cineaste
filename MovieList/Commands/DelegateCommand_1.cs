using System;
using System.Windows.Input;

namespace MovieList.Commands
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool> canExecute;

        public DelegateCommand(Action<T> execute)
            : this(execute, _ => true)
        { }

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(T parameter)
            => (this as ICommand).Execute(parameter);

        public bool CanExecute(T parameter)
            => (this as ICommand).CanExecute(parameter);

        void ICommand.Execute(object parameter)
        {
            if (parameter is T t)
            {
                this.execute(t);
            }
        }

        bool ICommand.CanExecute(object parameter)
            => parameter is T t && this.canExecute(t);
    }
}
