namespace MyConsole.InputProvider;

public class ProgressBar : IInputProvider
{
    private readonly double _maximum;
    private readonly double _tolerance;
    private double _oldValue;
    private double _value;

    public double Value
    {
        get => _value;
        set
        {
            _value = (value, _maximum) switch
            {
                (< 0, _) => 0,
                var (v, m) when v > m => m,
                _ => value
            };
            if (Math.Abs(_oldValue - _value) > _tolerance)
            {
                _oldValue = _value;
                Updated?.Invoke(GetContextString());
            }
        }
    }

    public string ClearString { get; } = Esc.ClearCurrentLine;
    public int Height { get; } = 1;
    public Action<string>? Completed { get; set; }
    public Action<string>? Updated { get; set; }

    public ProgressBar() : this(1)
    {
    }

    public ProgressBar(double maximum)
    {
        _value = 0;
        _oldValue = 0;
        _maximum = maximum;
        _tolerance = maximum / 1000;
    }

    private string GetContextString()
    {
        int width = Console.WindowWidth - 12;
        if (width > 80 - 12)
            width = 80 - 12;
        int completeWidth = (int) (Value / _maximum * width);
        int incompleteWidth = width - completeWidth;

        return
            $"[{new string('#', completeWidth)}{new string(' ', incompleteWidth)}] {Value / _maximum:P0}";
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }
}