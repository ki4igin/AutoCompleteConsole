namespace MyConsole.InputProvider;

public class ProgressBar : IInputProvider
{
    private int _current;
    private int _maximum;

    public string ClearString { get; } = Esc.ClearCurrentLine;
    public int Height { get; } = 1;
    public Action<string>? Completed { get; set; }
    public Action<string>? Updated { get; set; }

    public ProgressBar(int maximum)
    {
        _current = 0;
        _maximum = maximum;
    }

    public void Increment()
    {
        _current++;
        string str = GetContextString();
        Updated?.Invoke(str);
    }

    private string GetContextString()
    {
        int width = Console.WindowWidth - 12;
        if (width > 80 - 12)
            width = 80 - 12;
        int completeWidth = (int)((double)_current / _maximum * width);
        int incompleteWidth = width - completeWidth;

        return $"[{new string('#', completeWidth)}{new string(' ', incompleteWidth)}] {(int)((double)_current / _maximum * 100)}%";
    }


    public void Clear()
    {
        throw new NotImplementedException();
    }
}