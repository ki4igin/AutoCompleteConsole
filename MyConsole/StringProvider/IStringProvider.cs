namespace MyConsole.StringProvider;

public interface IStringProvider
{
    public Action<string>? Completed { get; set; }
    public Action<string>? Updated { get; set; }
}