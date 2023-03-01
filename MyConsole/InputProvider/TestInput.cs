using System.Text;

namespace MyConsole.InputProvider;

public class TestInput : IInputProvider
{
    private readonly StringBuilder _text;
    public string ClearString { get; set; }
    public int Height { get; private set; }
    public Action<string>? Completed { get; set; }
    public Action<string>? Updated { get; set; }

    public TestInput()
    {
        _text = new("");
        Height = 1;
        ClearString = Esc.ClearCurrentLine;
    }
    public void Clear()
    {
        _text.Clear();
        Height = 1;
    }

    public void Write(string str)
    {
        _text.Append(str);
        Updated?.Invoke(_text.ToString());
    }
    
}