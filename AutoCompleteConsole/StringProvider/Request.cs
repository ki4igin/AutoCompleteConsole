using System.Text;

namespace AutoCompleteConsole.StringProvider;

public class Request : IStringProvider
{
    public record Context(string Title = "", string SubTitle = "", string Comment = "");

    public record Color(EscColor Title, EscColor SubTitle, EscColor ErrorMessage, EscColor Cursor, EscColor Comment)
    {
        public Color(EscColor title, EscColor errorMessage, EscColor cursor) :
            this(title, EscColor.Reset, errorMessage, cursor, EscColor.Reset)
        {
        }

        public Color() :
            this(EscColor.Reset, EscColor.Reset, EscColor.Reset, EscColor.Reverse, EscColor.Reset)
        {
        }
    }

    private const int MaxCountAttempts = 3;
    private int _countAttempts;
    private Context _context;
    private readonly Color _color;

    public Action<string>? Completed { get; set; }
    public Action<string>? Updated { get; set; }

    internal Request(Color color)
    {
        _color = color;
        _context = new();
        _countAttempts = MaxCountAttempts;
    }

    public string ReadLine(Context context, string defaultValue) =>
        ReadLine(context, _ => "", defaultValue);

    public string ReadLine(Context context, Func<string, string> validator, string defaultValue)
    {
        _context = context;
        _countAttempts = MaxCountAttempts;

        int cursorPosition = defaultValue.Length;
        StringBuilder sb = new(defaultValue);
        Updated?.Invoke(GetContextString(defaultValue, cursorPosition, validator(defaultValue)));

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                if (validator(sb.ToString()) == "")
                    break;

                if (--_countAttempts == 0)
                {
                    sb.Clear();
                    sb.Append(defaultValue);
                    break;
                }
            }

            if (keyInfo.Key == ConsoleKey.Escape)
            {
                sb.Clear();
                sb.Append(defaultValue);
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
                        0 or
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

            Updated?.Invoke(GetContextString(sb.ToString(), cursorPosition, validator(sb.ToString())));
        }

        Completed?.Invoke(sb.ToString());

        return sb.ToString();
    }

    private string GetContextString(string s, int i, string error)
    {
        (string title, string subTitle, string comment) = _context;

        s += " ";
        string strPrefix = s[..i];
        char ch = s[i];
        string strSuffix = s[(i + 1)..];

        StringBuilder sb = new();

        sb.Append(title.Color(_color.Title));
        if (subTitle != "")
            sb.Append(Environment.NewLine + subTitle.Color(_color.SubTitle));
        sb.Append(Environment.NewLine + strPrefix + ch.Color(_color.Cursor) + strSuffix);
        if (error != "")
            sb.Append(Environment.NewLine +
                      $"{error} [{_countAttempts}]".Color(_color.ErrorMessage));
        if (comment != "")
            sb.Append(Environment.NewLine + comment.Color(_color.Comment));

        string result = sb.ToString();
        return result;
    }
}