using MyConsole.StringProvider;

namespace MyConsole;

internal class Status
{
    private int _height;
    private string _clearString;

    private string Text { get; set; }
    public int Position { get; set; }

    public Action<string, int, int>? Changed { get; set; }

    public Status(int position)
    {
        _clearString = Esc.ClearCurrentLine;
        Position = position;
        Text = "";
    }

    public void AddInput(IStringProvider @string)
    {
        @string.Updated = Change;
        @string.Completed = _ => { Clear(); };
    }

    public string GetUpdateNewLineString() =>
        Esc.GetDownString(
            _clearString + Environment.NewLine + Text,
            Position - 1,
            _height + 1);

    private void Change(string str)
    {
        Text = str;
        string clearString = Esc.GetClearString(_height);
        _height = str.Split('\n').Length;
        Changed?.Invoke(clearString + str, Position, _height);
        _clearString = Esc.GetClearString(_height);
    }

    private void Clear() =>
        Change("");
}