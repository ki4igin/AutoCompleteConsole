namespace MyConsole.InputProvider;

public interface IInputProvider
{
    public Action<string>? Completed { get; set; }
    public Action<string>? Updated { get; set; }
}