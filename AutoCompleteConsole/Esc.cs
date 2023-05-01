using System.Text;

namespace AutoCompleteConsole;

public enum EscColor
{
    Reset = 0,
    Bright = 1,
    Dim = 2,
    Underline = 4,
    Blink = 5,
    Reverse = 7,
    Hidden = 8,
    
    ForegroundDarkBlack = 30,
    ForegroundDarkRed = 31,
    ForegroundDarkGreen = 32,
    ForegroundDarkYellow = 33,
    ForegroundDarkBlue = 34,
    ForegroundDarkMagenta = 35,
    ForegroundDarkCyan = 36,
    ForegroundDarkWhite = 37,

    BackgroundDarkBlack = 40,
    BackgroundDarkRed = 41,
    BackgroundDarkGreen = 42,
    BackgroundDarkYellow = 43,
    BackgroundDarkBlue = 44,
    BackgroundDarkMagenta = 45,
    BackgroundDarkCyan = 46,
    BackgroundDarkWhite = 47,

    ForegroundBlack = 90,
    ForegroundRed = 91,
    ForegroundGreen = 92,
    ForegroundYellow = 93,
    ForegroundBlue = 94,
    ForegroundMagenta = 95,
    ForegroundCyan = 96,
    ForegroundWhite = 97,

    BackgroundBlack = 100,
    BackgroundRed = 101,
    BackgroundGreen = 102,
    BackgroundYellow = 103,
    BackgroundBlue = 104,
    BackgroundMagenta = 105,
    BackgroundCyan = 106,
    BackgroundWhite = 107,
};

public static class Esc
{
    public const string CursorEnableBlinking = "\u001b[?12h";
    public const string CursorDisableBlinking = "\u001b[?12l";
    public const string CursorShow = "\u001b[?25h";
    public const string CursorHide = "\u001b[?25l";
    public const string CursorHome = "\u001b[H";
    public const string CursorSavePosition = "\u001b[s";
    public const string CursorRestorePosition = "\u001b[u";
    
    public const string Clear = "\u001b[2J";
    public const string ClearCurrentLine = "\u001b[2K";

    public const string ScreenBufferAlternative = "\u001b[?1049h";
    public const string ScreenBufferMain = "\u001b[?1049l";

    public static string Color(this string str, EscColor color) =>
        color switch
        {
            EscColor.Reset => str,
            _ => $"\u001b[{(int)color}m{str}\u001b[{(int)EscColor.Reset}m"
        };

    public static string Color(this char ch, EscColor color) =>
        Color(ch.ToString(), color);

    public static string CursorUp(int offset = 1) => $"\u001b[{offset}A";
    public static string CursorDown(int offset = 1) => $"\u001b[{offset}B";
    public static string CursorRight(int offset = 1) => $"\u001b[{offset}C";
    public static string CursorLeft(int offset = 1) => $"\u001b[{offset}D";
    public static string CursorPositionLeft(int position ) => $"\u001b[{position}G";
    public static string CursorPositionTop(int position ) => $"\u001b[{position}d";
    public static string CursorPosition(int left, int top) => $"\u001b[{left};{top}H";
    public static string InsertCharacter(int count) => $"\u001b[{count}@";
    public static string DeleteCharacter(int count) => $"\u001b[{count}P";
    public static string EraseCharacter(int count) => $"\u001b[{count}X";
    public static string InsertLine(int count) => $"\u001b[{count}L";
    public static string DeleteLine(int count) => $"\u001b[{count}M";
    public static string ScrollUp(int count) => $"\u001b[{count}S";
    public static string ScrollDown(int count) => $"\u001b[{count}T";

    public static string GetDownString(string str, int offset, int height) =>
        '\r' +
        // Esc.ClearCurrentLine +
        new string('\n', offset) +
        str +
        CursorUp(offset + height - 1);
    
    public static string GetClearString(int height)
    {
        StringBuilder sb = new(ClearCurrentLine);
        for (int i = 0; i < height - 1; i++)
        {
            sb.Append(Environment.NewLine + ClearCurrentLine);
        }

        if (height > 1)
            sb.Append(CursorUp(height - 1));

        return sb.ToString();
    }

    public static int GetHeightString(string str) =>
        str.Split('\n').Length;
}