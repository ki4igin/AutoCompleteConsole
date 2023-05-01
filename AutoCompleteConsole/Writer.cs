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
            // const string pattern = @"\u001b\[\d+m|\r";
            // string[] lines = Regex
            //         .Replace(str, pattern, "")
            //         .Split("\n");
            string[] lines = str.Split("\n");
            string lastLine = lines.Last().Replace("\r", "");
            (int newLeft, int offsetTop) = (_cursorPosition.left, 0);
            offsetTop += lines.Length - 1;
            if (lines.Length > 1)
                newLeft = 0;

            newLeft += lastLine.Length;
            if (newLeft > Console.WindowWidth)
            {
                offsetTop += newLeft / Console.WindowWidth;
                newLeft %= Console.WindowWidth;
            }

            if (offsetTop > 0)
            {
                strSuffix =
                    GetNewLineSuffixString() +
                    Esc.CursorPositionLeft(newLeft + 1);
            }

            Console.Write(str.Replace("\n", $"\n{Esc.ClearCurrentLine}").Color(color) + strSuffix);

            _cursorPosition = (newLeft, _cursorPosition.top + offsetTop);
        }
    }

    internal void WriteFirst(string str, EscColor color)
    {
        if (_cursorPosition.left != 0)
            str = Environment.NewLine + str;
        Write(str, color);
    }

    internal void WriteFirst(string str) =>
        WriteFirst(str, EscColor.Reset);

    internal void WriteFirstLine() =>
        WriteFirst(Environment.NewLine);

    internal void WriteFirstLine(string str) =>
        WriteFirst(str + Environment.NewLine);

    internal void WriteFirstLine(string str, EscColor color) =>
        WriteFirst(str + Environment.NewLine, color);

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