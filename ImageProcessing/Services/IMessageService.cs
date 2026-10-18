// IMessageService.cs
public interface IMessageService
{
    // true, если пользователь нажал OK
    bool Confirm(string text);
}