using System.Text.RegularExpressions;

namespace MyConsole;

public class Writer
{
    private readonly object _lockObj = new();
    private (int left, int top) _cursorPosition;

    public Func<string>? NewLineSuffixString { get; set; }

    public Writer()
    {
        _cursorPosition = (0, 0);
        NewLineSuffixString = () => "";
    }

    public void Write(string str)
    {
        lock (_lockObj)
        {
            string strSuffix = "";
            if (IsNewLine(str))
            {
                strSuffix =
                    NewLineSuffixString?.Invoke() +
                    Esc.CursorPositionLeft(_cursorPosition.left + 1);
            }

            Console.Write(str + strSuffix);
        }
    }

    public void Write(string str, EscColor color)
    {
        lock (_lockObj)
        {
            string strSuffix = "";
            int cursorPositionLeft = _cursorPosition.left;
            if (IsNewLine(str))
            {
                strSuffix =
                    NewLineSuffixString?.Invoke() +
                    Esc.CursorPositionLeft(_cursorPosition.left + 1);
            }

            Console.Write(str.Color(color));
            Console.Write(strSuffix);
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
                GetWriteDownString(str, offset, height) +
                Esc.CursorPositionLeft(_cursorPosition.left + 1);
            Console.Write(s);
        }
    }

    public string GetWriteDownString(string str, int offset, int height) =>
        '\r' +
        // Esc.CursorDown(offset) +
        new string('\n', offset) +
        str +
        Esc.CursorUp(offset + height - 1);


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
}