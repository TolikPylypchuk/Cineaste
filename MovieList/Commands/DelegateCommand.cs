using System;
using System.Windows.Input;

namespace MovieList.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public DelegateCommand(Action execute)
            : this (execute, () => true)
        { }

        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute()
            => (this as ICommand).Execute(null);

        public bool CanExecute()
            => (this as ICommand).CanExecute(null);

        void ICommand.Execute(object parameter)
            => this.execute();

        bool ICommand.CanExecute(object parameter)
            => this.canExecute();
    }
}
