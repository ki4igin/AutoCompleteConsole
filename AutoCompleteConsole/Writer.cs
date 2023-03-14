namespace AutoCompleteConsole;

internal class Writer
{
    private readonly object _lockObj = new();
    private (int left, int top) _cursorPosition;

    private readonly List<Status> _statuses;
    private string _statusText = "";

    internal Writer()
    {
        _statuses = new();
        _cursorPosition = (0, 0);
    }

    internal void Write(string str) =>
        Write(str, EscColor.Reset);

    internal void Write(string str, EscColor color)
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

    internal void WriteLine() =>
        Write(Environment.NewLine);
    internal void WriteLine(string str) =>
        Write(str + Environment.NewLine);

    internal void WriteLine(string str, EscColor color) =>
        Write(str + Environment.NewLine, color);

    internal Status CreateStatus()
    {
        Status status = new();
        _statuses.Add(status);
        status.Changed = OnStatusChanged;
        return status;
    }

    internal void DeleteStatus(Status status)
    {
        _statuses.Remove(status);
    }

    internal void Clear()
    {
        lock (_lockObj)
        {
            Console.Write(Esc.Clear);
            Console.Write(Esc.CursorPosition(0, 0));
            _cursorPosition = (0, 0);
        }
    }

    private void WriteDown(string str, int offset, int height)
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

    private void OnStatusChanged(string str)
    {
        string clearString = Esc.GetClearString(Esc.GetHeightString(_statusText));

        string text = string.Join(
            Environment.NewLine,
            _statuses
                .Where(status => status.Text != "")
                .Reverse()
                .Select(status => status.Text));

        _statusText = text;
        WriteDown(clearString + text, 2, Esc.GetHeightString(text));
    }


    private string GetNewLineSuffixString()
    {
        int height = Esc.GetHeightString(_statusText);
        string clearString = Esc.GetClearString(height);

        return Esc.GetDownString(clearString + Environment.NewLine + _statusText, 1, height + 1);
    }
}