namespace MyConsole;

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
    public const string Reset = "\u001b[0m";
    public const string Bright = "\u001b[1m";
    public const string Dim = "\u001b[2m";
    public const string Underline = "\u001b[4m";
    public const string Blink = "\u001b[5m";
    public const string Reverse = "\u001b[7m";
    public const string Hidden = "\u001b[8m";

    public const string CursorEnableBlinking = "\u001b[?12h";
    public const string CursorDisableBlinking = "\u001b[?12l";
    public const string CursorShow = "\u001b[?25h";
    public const string CursorHide = "\u001b[?25l";

    public static string Color(this string str, EscColor color) =>
        color switch
        {
            EscColor.Reset => str,
            _ => $"\u001b[{(int)color}m{str}{Reset}"
        };

    public static string Color(this char ch, EscColor color) =>
        color switch
        {
            EscColor.Reset => $"{ch}",
            _ => $"\u001b[{(int)color}m{ch}{Reset}"
        };


    public const string Clear = "\u001b[2J";
    public const string ClearCurrentLine = "\u001b[2K";
    public const string CursorHome = "\u001b[H";
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
    public const string CursorSavePosition = "\u001b[s";
    public const string CursorRestorePosition = "\u001b[u";
}