using AutoCompleteConsole.StringProvider;

namespace AutoCompleteConsole;

public static class Acc
{
    private static readonly AutoCompleteString _rd;
    private static readonly Writer _wr;
    private static readonly Dictionary<string, Action> _commands;
    private static readonly Status _rdStatus;

    static Acc()
    {
        NativeTerminal.EnableVirtualTerminalProcessing();

        Console.Write(Esc.ScreenBufferAlternative);
        Console.Write(Esc.CursorHide);
        Console.Write(Esc.CursorPosition(0, 0));
        AppDomain.CurrentDomain.ProcessExit += (_, _) => Console.Write(Esc.ScreenBufferMain);
        Console.CancelKeyPress += (_, _) => Console.Write(Esc.ScreenBufferMain);

        _commands = new()
        {
            ["clear"] = Clear,
            ["quit"] = Quit
        };

        _wr = new();
        _rd = new(_commands.Keys,
            new(
                EscColor.Reset,
                EscColor.BackgroundDarkMagenta
            ));

        _rdStatus = _wr.CreateStatus();
        _rdStatus.Add(_rd);
    }

    public static void AddKeyWords(string[] keyWords) =>
        _rd.AddKeyWords(keyWords);

    public static Selector CreateSelector(Selector.Color color)
    {
        Selector input = new(color);
        _rdStatus.Add(input);
        return input;
    }

    public static Request CreateRequest(Request.Color color)
    {
        Request input = new(color);
        _rdStatus.Add(input);
        return input;
    }

    public static ProgressBar CreateProgressBar()
    {
        ProgressBar progressBar = new();
        Status status = _wr.CreateStatus();
        status.Add(progressBar);

        // progressBar.Completed = _ => _wr.DeleteStatus(status);

        return progressBar;
    }

    public static Status CreateStatus() =>
        _wr.CreateStatus();

    public static void Write(string str) => _wr.Write(str);
    public static void Write(string str, EscColor color) => _wr.Write(str, color);
    public static void WriteLine() => _wr.WriteLine();
    public static void WriteLine(string str) => _wr.WriteLine(str);
    public static void WriteLine(string str, EscColor color) => _wr.WriteLine(str, color);

    public static string ReadLine()
    {
        string cmd = _rd.ReadLine();
        if (_commands.TryGetValue(cmd, out var action))
            action.Invoke();

        return cmd;
    }

    private static void Quit() =>
        Environment.Exit(0);

    private static void Clear() =>
        _wr.Clear();
}