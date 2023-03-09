using System.Runtime.InteropServices;
using MyConsole.StringProvider;

namespace MyConsole;

public class MainConsole
{
    private readonly AutoCompleteString _rd;
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
        _rd = new(_commands.Keys,
            new(
                EscColor.Reset,
                EscColor.BackgroundDarkMagenta
            ));

        _rdStatus = _wr.CreateStatus();
        _rdStatus.AddInput(_rd);
    }

    public Selector CreateSelector(Selector.Color color)
    {
        Selector input = new(color);
        _rdStatus.AddInput(input);
        return input;
    }

    public Request CreateRequest(Request.Color color)
    {
        Request input = new(color);
        _rdStatus.AddInput(input);
        return input;
    }
    
    public ProgressBar CreateProgressBar()
    {
        ProgressBar progressBar = new();
        Status status = _wr.CreateStatus();
        status.AddInput(progressBar);
    
        progressBar.Completed = _ => _wr.DeleteStatus(status);
    
        return progressBar;
    }

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