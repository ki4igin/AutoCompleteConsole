namespace MyConsole.InputProvider;

public interface IColors
{
}

public interface IInputProvider<in T> where T : IColors
{
    public void SetColors(T colors);
    public Action<string>? Completed { get; set; }
    public Action<string>? Updated { get; set; }
}