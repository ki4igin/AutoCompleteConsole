using System.Text;

namespace AutoCompleteConsole.StringProvider;

public class TestStringProvider : IStringProvider
{
    private readonly StringBuilder _text;
    public Action<string>? Completed { get; set; }
    public Action<string>? Updated { get; set; }

    internal TestStringProvider()
    {
        _text = new("");
    }

    public void Write(string str)
    {
        _text.Append(str);
        Updated?.Invoke(_text.ToString());
    }
}