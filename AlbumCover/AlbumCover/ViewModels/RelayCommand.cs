using System;
using System.Windows.Input;

namespace AlbumCover.ViewModels
{
    class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;


        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
               
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        // [출처] View의 Event를 ViewModel에서 핸들링하기 - 개요|작성자 에로로
    }
}
