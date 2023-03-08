using System.Reflection;
using System.Text;

namespace MyConsole.InputProvider;

public class Selector : IInputProvider
{
    private const int StartListNumber = 1;

    private int _selectPosition;

    private string Title { get; }
    private string SubTitle { get; set; }
    private string[] Items { get; }
    private string Comment { get; set; }
    private EscColor TitleColor { get; set; }
    private EscColor SubTitleColor { get; set; }
    private EscColor ItemsColor { get; set; }
    private EscColor SelectColor { get; set; }
    private EscColor CommentColor { get; set; }
    public Action<string>? Updated { get; set; }
    public Action<string>? Completed { get; set; }

    private Selector(string title, string[] items, int selectPosition = 1)
    {
        Title = title;
        SubTitle = "";
        Items = items;
        Comment = "";
        TitleColor = EscColor.Reset;
        SubTitleColor = EscColor.Reset;
        ItemsColor = EscColor.Reset;
        SelectColor = EscColor.Reverse;
        CommentColor = EscColor.Reset;

        _selectPosition = selectPosition - 1;
    }

    public static SelectorBuilder Create(string title, string[] items, int selectPosition = 1) =>
        new(new(title, items, selectPosition));
    
    public string Run()
    {
        Updated?.Invoke(GetContextString());
        int itemsLength = Items.Length;
        int startSelectPosition = _selectPosition;
        ConsoleKey key;
        do
        {
            int oldStrPos = _selectPosition;
            key = Console.ReadKey(true).Key;

            _selectPosition = key switch
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
            if (oldStrPos == _selectPosition)
                continue;

            Updated?.Invoke(GetContextString());
        } while (key is not ConsoleKey.Enter and not ConsoleKey.Escape);

        Completed?.Invoke(Items[_selectPosition]);

        return Items[_selectPosition];
    }

    private string GetContextString()
    {
        StringBuilder sb = new();
        sb.Append(Title.Color(TitleColor));
        if (SubTitle != "")
            sb.Append(Environment.NewLine + SubTitle.Color(SubTitleColor));
        int alignment = Items.Select(s => s.Length).Max() + $"[{Items.Length}] ".Length;
        for (int i = 0; i < Items.Length; i++)
        {
            sb.Append(Environment.NewLine);
            string itemStr = $"[{i + StartListNumber}] {Items[i]}".PadRight(alignment);
            sb.Append(i == _selectPosition ? itemStr.Color(SelectColor) : itemStr.Color(ItemsColor));
        }

        if (Comment != "")
            sb.Append(Environment.NewLine + Comment.Color(CommentColor));

        return sb.ToString();
    }

    public class SelectorBuilder
    {
        private readonly Selector _selector;

        internal SelectorBuilder(Selector selector)
            => _selector = selector;

        public SelectorBuilder BaseColor(EscColor color)
        {
            EscColor selectColor = _selector.SelectColor;
            foreach (PropertyInfo prop in typeof(Selector).GetProperties())
            {
                if (prop.PropertyType == typeof(EscColor))
                {
                    prop.SetValue(_selector, color);
                }
            }

            _selector.SelectColor = selectColor;
            return this;
        }

        public SelectorBuilder TitleColor(EscColor color)
        {
            _selector.TitleColor = color;
            return this;
        }

        public SelectorBuilder SubTitleColor(EscColor color)
        {
            _selector.SubTitleColor = color;
            return this;
        }

        public SelectorBuilder ItemsColor(EscColor color)
        {
            _selector.ItemsColor = color;
            return this;
        }

        public SelectorBuilder SelectColor(EscColor color)
        {
            _selector.SelectColor = color;
            return this;
        }

        public SelectorBuilder CommentColor(EscColor color)
        {
            _selector.CommentColor = color;
            return this;
        }

        public SelectorBuilder SubTitle(string str)
        {
            _selector.SubTitle = str;
            return this;
        }

        public SelectorBuilder Comment(string str)
        {
            _selector.Comment = str;
            return this;
        }

        public Selector Build() =>
            _selector;
    }
}