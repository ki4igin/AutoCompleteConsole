using MyConsole.InputProvider;

namespace MyConsole;

public class Status : IDisposable
{
    private readonly IInputProvider _inputProvider;
    
    private readonly Writer _writer;

    public string Text { get; private set; }
    public string ClearString => _inputProvider.ClearString;
    public int Height => _inputProvider.Height;

    public int Position { get; }

    private readonly EscColor _color;

    private readonly Action<Status> _disposeAction;

    public Status(Writer writer, Action<Status> disposeAction, IInputProvider inputProvider, int position, EscColor color = EscColor.Reset)
    {
        _disposeAction = disposeAction;
        _writer = writer;
        Position = position;
        _color = color;
        Text = "";
        
        _inputProvider = inputProvider;
        _inputProvider.Updated = Write;
        _inputProvider.Completed = _ =>
        {
            Clear();
        };
    }

    public void Write(string str)
    {
        Text = str.Color(_color);
        _writer.WriteDown(_inputProvider.ClearString + Text, Position, _inputProvider.Height);
    }

    private void Clear()
    {
        Text = "";
        _writer.WriteDown(_inputProvider.ClearString, Position, 1);
        _inputProvider.Clear();
    }

    public void Dispose()
    {
        Clear();
        _disposeAction.Invoke(this);
    }
}