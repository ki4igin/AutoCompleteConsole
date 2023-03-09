using System.Text;

namespace MyConsole.InputProvider;

public class TestInput : IInputProvider
{
    private readonly StringBuilder _text;
    public Action<string>? Completed { get; set; }
    public Action<string>? Updated { get; set; }

    internal TestInput()
    {
        _text = new("");
    }

    public void Write(string str)
    {
        _text.Append(str);
        Updated?.Invoke(_text.ToString());
    }
    
}