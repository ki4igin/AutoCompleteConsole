using MyConsole.InputProvider;

namespace MyConsole;

public class Status : IDisposable
{
    private readonly IInputProvider _inputProvider;

    private readonly Writer _writer;

    private string Text { get; set; }

    public int Position { get; }

    private readonly Action<Status> _disposeAction;

    public Status(
        Writer writer,
        Action<Status> disposeAction,
        IInputProvider inputProvider,
        int position)
    {
        _disposeAction = disposeAction;
        _writer = writer;
        Position = position;
        Text = "";

        _inputProvider = inputProvider;
        _inputProvider.Updated = Change;
        _inputProvider.Completed = _ => { Clear(); };
    }

    public string GetUpdateNewLineString() =>
        Esc.GetDownString(
            _inputProvider.ClearString + Environment.NewLine + Text,
            Position - 1,
            _inputProvider.Height + 1);

    public void Dispose()
    {
        Clear();
        _disposeAction.Invoke(this);
    }

    private void Change(string str)
    {
        Text = str;
        _writer.WriteDown(_inputProvider.ClearString + Text, Position, _inputProvider.Height);
    }

    private void Clear()
    {
        Text = "";
        _writer.WriteDown(_inputProvider.ClearString, Position, 1);
        _inputProvider.Clear();
    }
}