using System.Text;
using MyConsole.InputProvider;

namespace MyConsole;

internal class Status
{
    private int _height;
    private string _clearString;

    private string Text { get; set; }
    public int Position { get; }

    public Action<string, int, int>? Changed { get; set; }

    public Status(int position)
    {
        _clearString = Esc.ClearCurrentLine;
        Position = position;
        Text = "";
    }

    public void AddInput(IInputProvider input)
    {
        input.Updated = Change;
        input.Completed = _ => { Clear(); };
    }

    public string GetUpdateNewLineString() =>
        Esc.GetDownString(
            _clearString + Environment.NewLine + Text,
            Position - 1,
            _height + 1);

    private void Change(string str)
    {
        Text = str;
        string clearString = GetClearString(_height);
        _height = str.Split('\n').Length;
        Changed?.Invoke(clearString + str, Position, _height);
        _clearString = GetClearString(_height);
    }

    private void Clear() =>
        Change("");


    private string GetClearString(int height)
    {
        StringBuilder sb = new(Esc.ClearCurrentLine);
        for (int i = 0; i < height - 1; i++)
        {
            sb.Append(Environment.NewLine + Esc.ClearCurrentLine);
        }

        if (height > 1)
            sb.Append(Esc.CursorUp(height - 1));

        return sb.ToString();
    }
}