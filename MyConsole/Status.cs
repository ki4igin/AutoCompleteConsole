using MyConsole.InputProvider;

namespace MyConsole;

public class Status
{
    private IInputProvider _inputProvider;

    private readonly Writer _writer;

    public string Text { get; private set; }
    public string ClearString => _inputProvider.ClearString;
    public int Height => _inputProvider.Height;

    public int Position { get; }

    private EscColor _color;

    public Status(Writer writer, IInputProvider inputProvider, int position, EscColor color = EscColor.Reset)
    {
        _writer = writer;
        Position = position;
        _color = color;
        Text = "";
        
        _inputProvider = inputProvider;
        Redirect(inputProvider);
    }

    public void Redirect(IInputProvider inputProvider)
    {
        _inputProvider = inputProvider;
        _inputProvider.Updated = Write;
        _inputProvider.Completed = s =>
        {
            Clear();
            _writer.WriteLine(Environment.NewLine + s, _color);
        };
    }

    public void Write(string str)
    {
        Text = _color switch 
        {
            EscColor.Reset => str,
            _ => str.Color(_color)
        };
        _writer.WriteDown(_inputProvider.ClearString + Text, Position, _inputProvider.Height);
    }

    public void Clear()
    {
        Text = "";
        _writer.WriteDown(_inputProvider.ClearString, Position, 1);
        _inputProvider.Clear();
    }

    public string GetUpdateString() =>
        _writer.GetWriteDownString(
            ClearString +
            Environment.NewLine +
            Text,
            Position - 1,
            Height + 1);
}