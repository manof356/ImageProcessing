// MessageService.cs
using System.Windows;

public class MessageService : IMessageService
{
    public bool Confirm(string text) =>
        MessageBox.Show(text, "Экспорт", MessageBoxButton.OKCancel,
                        MessageBoxImage.Warning) == MessageBoxResult.OK;
}