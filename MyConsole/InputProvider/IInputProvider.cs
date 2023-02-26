namespace MyConsole.InputProvider;

public interface IInputProvider
{
    public string ClearString { get; }
    public int Height { get; }
    
    public Action<string>? Completed { get; set; }
    public Action<string>? Updated { get; set; }

    public void Clear();
}