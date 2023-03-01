using System.Reflection;
using System.Text;

namespace MyConsole.InputProvider;

public class Request : IInputProvider
{
    private readonly Predicate<string> _validator;
    private readonly string _defaultValue;
    private int _maxCountAttempts;
    private string Title { get; set; }
    private string SubTitle { get; set; }
    private string ErrorMessage { get; set; }
    private string Comment { get; set; }
    private EscColor TitleColor { get; set; }
    private EscColor SubTitleColor { get; set; }
    private EscColor ErrorMessageColor { get; set; }
    private EscColor CursorColor { get; set; }
    private EscColor CommentColor { get; set; }
    public string ClearString { get; private set; }
    public int Height { get; private set; }
    public Action<string>? Completed { get; set; }
    public Action<string>? Updated { get; set; }

    private Request(string title, Predicate<string>? validator = default, string defaultValue = "")
    {
        Title = title;
        SubTitle = "";
        ErrorMessage = "Error";
        Comment = "";
        TitleColor = EscColor.Reset;
        SubTitleColor = EscColor.Reset;
        ErrorMessageColor = EscColor.Reset;
        CursorColor = EscColor.BackgroundDarkMagenta;
        CommentColor = EscColor.Reset;

        ClearString = Esc.ClearCurrentLine;
        Height = 1;

        _validator = validator ?? (_ => true);
        _defaultValue = defaultValue;
        _maxCountAttempts = 3;
    }

    public static RequestBuilder Create(string title, Predicate<string>? validator = null, string defaultValue = "") =>
        new(new(title, validator, defaultValue));

    public string Run()
    {
        int cursorPosition = _defaultValue.Length;
        StringBuilder sb = new(_defaultValue);
        Updated?.Invoke(GetContextString(_defaultValue, cursorPosition));
        ClearString = GetClearString();

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                if (_validator(sb.ToString()))
                    break;

                if (--_maxCountAttempts == 0)
                {
                    sb.Clear();
                    sb.Append(_defaultValue);
                    break;
                }
            }

            if (keyInfo.Key == ConsoleKey.Escape)
            {
                sb.Clear();
                sb.Append(_defaultValue);
                break;
            }

            switch (keyInfo.Key)
            {
                case ConsoleKey.Backspace:
                    if (cursorPosition != 0)
                    {
                        cursorPosition--;
                        sb.Remove(cursorPosition, 1);
                    }

                    break;
                case ConsoleKey.Delete:
                    if (cursorPosition != sb.Length)
                    {
                        sb.Remove(cursorPosition, 1);
                    }

                    break;
                case ConsoleKey.LeftArrow:
                    if (cursorPosition != 0)
                    {
                        cursorPosition--;
                    }

                    break;
                case ConsoleKey.RightArrow:
                    if (cursorPosition != sb.Length)
                    {
                        cursorPosition++;
                    }

                    break;
                case ConsoleKey.End:
                    cursorPosition = sb.Length;
                    break;
                case ConsoleKey.Home:
                    cursorPosition = 0;
                    break;
                default:
                    if (keyInfo.Key is
                        >= ConsoleKey.D0 and <= ConsoleKey.Z or
                        >= ConsoleKey.NumPad0 and <= ConsoleKey.Divide or
                        >= ConsoleKey.Oem1 and <= ConsoleKey.Oem102 or
                        ConsoleKey.Spacebar)
                    {
                        sb.Insert(cursorPosition, keyInfo.KeyChar);
                        cursorPosition++;
                    }

                    break;
            }

            Updated?.Invoke(GetContextString(sb.ToString(), cursorPosition));
            ClearString = GetClearString();
        }

        Completed?.Invoke(sb.ToString());

        return sb.ToString();
    }

    public void Clear()
    {
        ClearString = Esc.ClearCurrentLine;
        Height = 1;
    }

    private string GetContextString(string s, int i)
    {
        s += " ";
        string strPrefix = s[..i];
        char ch = s[i];
        string strSuffix = s[(i + 1)..];

        StringBuilder sb = new();

        sb.Append(Title.Color(TitleColor));
        if (SubTitle != "")
            sb.Append(Environment.NewLine + SubTitle.Color(SubTitleColor));
        if (_validator(s) is false)
            sb.Append(Environment.NewLine +
                      $"{ErrorMessage} [{_maxCountAttempts}]".Color(ErrorMessageColor));
        sb.Append(Environment.NewLine + strPrefix + ch.Color(CursorColor) + strSuffix);
        if (Comment != "")
            sb.Append(Environment.NewLine + Comment.Color(CommentColor));

        string result = sb.ToString();
        Height = result.Split('\n').Length;
        return result;
    }

    private string GetClearString()
    {
        StringBuilder sb = new(Esc.ClearCurrentLine);
        for (int i = 0; i < Height - 1; i++)
        {
            sb.Append(Environment.NewLine + Esc.ClearCurrentLine);
        }

        sb.Append(Esc.CursorUp(Height - 1));
        return sb.ToString();
    }

    public class RequestBuilder
    {
        private readonly Request _request;

        internal RequestBuilder(Request request) =>
            _request = request;

        public Request Build() =>
            _request;

        public RequestBuilder BaseColor(EscColor color)
        {
            EscColor cursorColor = _request.CursorColor;
            foreach (PropertyInfo prop in typeof(Selector).GetProperties())
            {
                if (prop.PropertyType == typeof(EscColor))
                {
                    prop.SetValue(_request, color);
                }
            }

            _request.CursorColor = cursorColor;
            return this;
        }

        public RequestBuilder TitleColor(EscColor color)
        {
            _request.TitleColor = color;
            return this;
        }

        public RequestBuilder SubTitleColor(EscColor color)
        {
            _request.SubTitleColor = color;
            return this;
        }

        public RequestBuilder ErrorMessageColor(EscColor color)
        {
            _request.ErrorMessageColor = color;
            return this;
        }

        public RequestBuilder CursorColor(EscColor color)
        {
            _request.CursorColor = color;
            return this;
        }

        public RequestBuilder CommentColor(EscColor color)
        {
            _request.CommentColor = color;
            return this;
        }

        public RequestBuilder SubTitle(string str)
        {
            _request.SubTitle = str;
            return this;
        }

        public RequestBuilder ErrorMessage(string str)
        {
            _request.ErrorMessage = str;
            return this;
        }

        public RequestBuilder Comment(string str)
        {
            _request.Comment = str;
            return this;
        }
    }
}