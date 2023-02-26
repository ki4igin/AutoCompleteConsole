namespace MyConsole.InputProvider;

public class TestInput : IInputProvider
{
    public string ClearString => Esc.ClearCurrentLine;
    public int Height => 1;
    public Action<string>? Completed { get; set; }
    public Action<string>? Updated { get; set; }
    public void Clear()
    {
    }
}