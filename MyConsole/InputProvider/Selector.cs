using System.Text;

namespace MyConsole.InputProvider;

public class Selector : IInputProvider
{
    public record Context(string Title, string SubTitle, string[] Items, string Comment)
    {
        public Context(string title, string[] items) : this(title, "", items, "")
        {
        }

        internal Context() : this("", "", new[] {""}, "")
        {
        }
    }

    public record Color(EscColor Title, EscColor SubTitle, EscColor Items, EscColor Select, EscColor Comment)
    {
        internal Color() : this(EscColor.Reset, EscColor.Reset, EscColor.Reset, EscColor.Reverse, EscColor.Reset)
        {
        }
    }

    private const int StartListNumber = 1;
    private Context _context;
    private readonly Color _color;

    public Action<string>? Updated { get; set; }
    public Action<string>? Completed { get; set; }

    public Selector() : this(new())
    {
    }

    public Selector(Color color)
    {
        _color = color;
        _context = new();
    }

    public string Run(Context context, int defaultSelectPosition = 1)
    {
        _context = context;
        int startSelectPosition = defaultSelectPosition - 1;

        Updated?.Invoke(GetContextString(startSelectPosition));
        int itemsLength = _context.Items.Length;

        int selectPosition = startSelectPosition;
        ConsoleKey key;
        do
        {
            int oldStrPos = selectPosition;
            key = Console.ReadKey(true).Key;

            selectPosition = key switch
            {
                ConsoleKey.UpArrow => (oldStrPos == 0) ? (itemsLength - 1) : (oldStrPos - 1),
                ConsoleKey.DownArrow => (oldStrPos == itemsLength - 1) ? (0) : (oldStrPos + 1),
                ConsoleKey.Escape => startSelectPosition,
                ConsoleKey.Backspace => startSelectPosition,
                >= ConsoleKey.D0 and <= ConsoleKey.D9 when
                    key - ConsoleKey.D0 >= StartListNumber &&
                    key - ConsoleKey.D0 <= StartListNumber + itemsLength - 1 =>
                    key - ConsoleKey.D0 - StartListNumber,
                >= ConsoleKey.NumPad0 and <= ConsoleKey.NumPad9 when
                    key - ConsoleKey.NumPad0 >= StartListNumber &&
                    key - ConsoleKey.NumPad0 <= StartListNumber + itemsLength - 1 =>
                    key - ConsoleKey.NumPad0 - StartListNumber,
                _ => oldStrPos
            };
            if (oldStrPos == selectPosition)
                continue;

            Updated?.Invoke(GetContextString(selectPosition));
        } while (key is not ConsoleKey.Enter and not ConsoleKey.Escape);

        string selectItem = _context.Items[selectPosition];
        Completed?.Invoke(selectItem);
        return selectItem;
    }

    private string GetContextString(int selectPosition)
    {
        (string title, string subTitle, string[] items, string comment) = _context;
        StringBuilder sb = new();
        sb.Append(title.Color(_color.Title));
        if (subTitle != "")
            sb.Append(Environment.NewLine + subTitle.Color(_color.SubTitle));
        int alignment = items.Select(s => s.Length).Max() + $"[{items.Length}] ".Length;
        for (int i = 0; i < items.Length; i++)
        {
            sb.Append(Environment.NewLine);
            string itemStr = $"[{i + StartListNumber}] {items[i]}".PadRight(alignment);
            sb.Append(i == selectPosition ? itemStr.Color(_color.Select) : itemStr.Color(_color.Items));
        }

        if (comment != "")
            sb.Append(Environment.NewLine + comment.Color(_color.Comment));

        return sb.ToString();
    }
}