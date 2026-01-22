using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace PhotoFrame
{
    public class RelayCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private readonly Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => _execute();
    }
}
