using System.Text;

namespace MyConsole.StringProvider;

public class TestString : IStringProvider
{
    private readonly StringBuilder _text;
    public Action<string>? Completed { get; set; }
    public Action<string>? Updated { get; set; }

    internal TestString()
    {
        _text = new("");
    }

    public void Write(string str)
    {
        _text.Append(str);
        Updated?.Invoke(_text.ToString());
    }
    
}