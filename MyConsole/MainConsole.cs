using MyConsole.InputProvider;

namespace MyConsole;

public class MainConsole
{
    private readonly AutoCompleteInput _rd;
    private readonly Writer _wr;
    private readonly Dictionary<string, Action> _commands;
    private readonly Status _rdStatus;

    public MainConsole()
    {
        NativeTerminal.EnableVirtualTerminalProcessing();

        Console.Write(Esc.ScreenBufferAlternative);
        Console.Write(Esc.CursorPosition(0, 0));
        AppDomain.CurrentDomain.ProcessExit += (_, _) => Console.Write(Esc.ScreenBufferMain);
        Console.CancelKeyPress += (_, _) => Console.Write(Esc.ScreenBufferMain);

        _commands = new()
        {
            ["clear"] = Clear,
            ["quit"] = Quit
        };

        _wr = new();
        _rd = AutoCompleteInput
            .With(_commands.Keys)
            .TextColor(EscColor.ForegroundBlue)
            .CursorColor(EscColor.BackgroundDarkMagenta)
            .Build();

        _rdStatus = _wr.CreateStatus(3);
        _rdStatus.AddInput(_rd);
    }

    public void AddInputToStatus(IInputProvider input) =>
        _rdStatus.AddInput(input);

    // public IDisposable CreateStatus(IInputProvider input) =>
    //     input switch
    //     {
    //         ProgressBar => _wr.CreateStatus(2),
    //         _ => _wr.CreateStatus(3)
    //     };

    public void Write(string str) => _wr.Write(str);
    public void Write(string str, EscColor color) => _wr.Write(str, color);
    public void WriteLine(string str) => _wr.WriteLine(str);
    public void WriteLine(string str, EscColor color) => _wr.WriteLine(str, color);

    public string ReadLine()
    {
        string cmd = _rd.ReadLine();
        if (_commands.TryGetValue(cmd, out var action))
            action.Invoke();

        return cmd;
    }


    private static void Quit() =>
        Environment.Exit(0);

    private void Clear() =>
        _wr.Clear();
}