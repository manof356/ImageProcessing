using System;
using System.Windows.Input;

namespace ImageProcessing.Common
{
    // Превращает обычный метод в команду, которую можно привязать к кнопке
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;          // что выполнить
        private readonly Func<bool> _canExecute;   // можно ли выполнять (необязательно)

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Кнопка спрашивает: "ты сейчас доступна?"
        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        // Кнопка нажата, вызываем метод
        public void Execute(object parameter) => _execute();

        // WPF сам перепроверяет CanExecute, когда в интерфейсе что-то меняется
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
