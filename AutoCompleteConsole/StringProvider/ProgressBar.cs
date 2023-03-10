namespace AutoCompleteConsole.StringProvider;

public class
    ProgressBar : IStringProvider, IProgress<double>
{
    private const double Tolerance = 0.001;

    private double _oldValue;
    private double _value;

    public Action<string>? Completed { get; set; }
    public Action<string>? Updated { get; set; }

    internal ProgressBar()
    {
        _value = 0;
        _oldValue = 0;
    }

    private string GetContextString()
    {
        int width = Console.WindowWidth - 12;
        if (width > 80 - 12)
            width = 80 - 12;
        int completeWidth = (int) (_value * width);
        int incompleteWidth = width - completeWidth;

        return _value switch
        {
            < Tolerance => "",
            > 1 - Tolerance => "Done",
            _ => $"[{new('#', completeWidth)}{new(' ', incompleteWidth)}] {_value:P0}"
        };
    }

    public void Report(double value)
    {
        _value = value switch
        {
            < 0 => 0,
            > 1 => 1,
            _ => value
        };
        if ((Math.Abs(_oldValue - _value) > Tolerance) is false)
            return;
        _oldValue = _value;
        Updated?.Invoke(GetContextString());
        if (_value > 1 - Tolerance)
        {
            Completed?.Invoke("Done");
        }
    }
}