using System.Text;
using System.Text.RegularExpressions;
using MyConsole.InputProvider;

namespace MyConsole;

public class Writer
{
    private readonly object _lockObj = new();
    private (int left, int top) _cursorPosition;

    private readonly List<Status> _statuses;

    public Writer()
    {
        _statuses = new();
        _cursorPosition = (0, 0);
    }

    public void Write(string str) =>
        Write(str, EscColor.Reset);

    public void Write(string str, EscColor color)
    {
        lock (_lockObj)
        {
            string strSuffix = "";
            if (IsNewLine(str))
            {
                strSuffix =
                    GetNewLineSuffixString() +
                    Esc.CursorPositionLeft(_cursorPosition.left + 1);
            }

            Console.Write(str.Color(color) + strSuffix);
        }
    }

    public void WriteLine(string str) =>
        Write(str + Environment.NewLine);

    public void WriteLine(string str, EscColor color) =>
        Write(str + Environment.NewLine, color);

    public void WriteDown(string str, int offset)
    {
        int height = str.Split('\n').Length;
        WriteDown(str, offset, height);
    }

    public void WriteDown(string str, int offset, int height)
    {
        lock (_lockObj)
        {
            string s =
                Esc.GetDownString(str, offset, height) +
                Esc.CursorPositionLeft(_cursorPosition.left + 1);
            Console.Write(s);
        }
    }

    private bool IsNewLine(string str)
    {
        bool isNewLine = false;
        // const string pattern = @"\u001b\[\d+m|\r";
        // string[] lines = Regex
        //         .Replace(str, pattern, "")
        //         .Split("\n");
        string[] lines = str
            .Replace("\r", "")
            .Split("\n");
        string lastLine = lines.Last();
        if (lines.Length > 1)
        {
            _cursorPosition.top += lines.Length - 1;
            _cursorPosition.left = 0;
            isNewLine = true;
        }

        _cursorPosition.left += lastLine.Length;
        if (_cursorPosition.left > Console.WindowWidth)
        {
            _cursorPosition.top += _cursorPosition.left / Console.WindowWidth;
            _cursorPosition.left %= Console.WindowWidth;
            isNewLine = true;
        }

        return isNewLine;
    }

    public Status CreateStatus(int position, IInputProvider inputProvider)
    {
        Status status = new(this, s => _statuses.Remove(s), inputProvider, position);
        _statuses.Add(status);
        return status;
    }

    private string GetNewLineSuffixString()
    {
        StringBuilder sb = new();
        foreach (var status in _statuses.OrderByDescending(s => s.Position))
        {
            string str = status.GetUpdateNewLineString(); 
            sb.Append(str);
        }

        return sb.ToString();
    }
}