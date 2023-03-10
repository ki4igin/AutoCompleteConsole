using AutoCompleteConsole.StringProvider;

namespace AutoCompleteConsole;

public class Status
{
    public string Text { get; private set; }

    internal Action<string>? Changed { get; set; }

    internal Status()
    {
        Text = "";
    }

    public void Change(string str)
    {
        Text = str;
        Changed?.Invoke(str);
    }

    internal void Add(IStringProvider @string)
    {
        @string.Updated = Change;
        @string.Completed = _ => { Clear(); };
    }

    private void Clear() =>
        Change("");
}