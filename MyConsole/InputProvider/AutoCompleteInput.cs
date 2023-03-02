using System.Text;

namespace MyConsole.InputProvider;

internal class AutoCompleteInput : IInputProvider
{
    private readonly List<string> _keyWords;
    private readonly List<string> _history;
    
    private EscColor TextColor { get; set; }
    private EscColor CursorColor { get; set; }

    public Action<string>? Updated { get; set; }
    public Action<string>? Completed { get; set; }

   
    public int Height { get; private set; } = 1;

    private AutoCompleteInput(IEnumerable<string> keyWords)
    {
        _history = new();
        _keyWords = new(keyWords);

        TextColor = EscColor.Reset;
        CursorColor = EscColor.BackgroundDarkMagenta;
    }

    public static AutoCompleteInputBuilder With(IEnumerable<string> keyWords) =>
        new(new(keyWords));
    

    public void AddKeyWords(IEnumerable<string> keyWords)
    {
        _keyWords.AddRange(keyWords);
    }

    public string ReadLine()
    {
        StringBuilder sb = new();

        int tabCount = 0;
        int historyIndex = _history.Count;
        int cursorPosition = 0;

        Updated?.Invoke(GetContextString("", 0));

        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.Enter && sb.Length > 0)
            {
                break;
            }

            if (keyInfo.Key != ConsoleKey.Tab)
            {
                tabCount = 0;
            }

            switch (keyInfo.Key)
            {
                case ConsoleKey.Backspace:
                    if (cursorPosition != 0)
                    {
                        cursorPosition--;
                        sb.Remove(cursorPosition, 1);
                    }

                    break;
                case ConsoleKey.Delete:
                    if (cursorPosition != sb.Length)
                    {
                        sb.Remove(cursorPosition, 1);
                    }

                    break;
                case ConsoleKey.LeftArrow:
                    if (cursorPosition != 0)
                    {
                        cursorPosition--;
                    }

                    break;
                case ConsoleKey.RightArrow:
                    if (cursorPosition != sb.Length)
                    {
                        cursorPosition++;
                    }

                    break;
                case ConsoleKey.UpArrow:
                    if (historyIndex != 0)
                    {
                        historyIndex--;
                        sb.Clear();
                        sb.Append(_history[historyIndex]);
                        cursorPosition = sb.Length;
                    }

                    break;
                case ConsoleKey.DownArrow:
                    historyIndex++;
                    if (historyIndex < _history.Count)
                    {
                        sb.Clear();
                        sb.Append(_history[historyIndex]);
                        cursorPosition = sb.Length;
                    }
                    else
                    {
                        historyIndex--;
                    }

                    break;
                case ConsoleKey.End:
                    cursorPosition = sb.Length;
                    break;
                case ConsoleKey.Home:
                    cursorPosition = 0;
                    break;
                case ConsoleKey.Tab:
                    string[] words = _keyWords
                        .Where(word => word.StartsWith(sb.ToString()))
                        .ToArray();
                    switch (words.Length)
                    {
                        case 0:
                            break;
                        case 1:
                            sb.Clear();
                            sb.Append(words.First());
                            break;
                        default:
                        {
                            string commonPrefix = words
                                .Aggregate((s1, s2) => string.Concat(s1.Zip(s2, (c1, c2) => c1 == c2 ? c1 : default)))
                                .TrimEnd('\0');
                            string all = AllKeyWordsToString(commonPrefix);

                            sb.Clear();
                            sb.Append(commonPrefix);
                            if ((tabCount++ & 0x01) == 0)
                            {
                                Updated?.Invoke(GetContextString(all, all.Length));
                                UpdateClearString(all);
                                cursorPosition = sb.Length;
                                continue;
                            }

                            break;
                        }
                    }

                    cursorPosition = sb.Length;
                    break;
                case ConsoleKey.Enter:
                    break;
                default:
                    if (keyInfo.Key is
                        >= ConsoleKey.D0 and <= ConsoleKey.Z or
                        >= ConsoleKey.NumPad0 and <= ConsoleKey.Divide or
                        >= ConsoleKey.Oem1 and <= ConsoleKey.Oem102 or
                        ConsoleKey.Spacebar)
                    {
                        sb.Insert(cursorPosition, keyInfo.KeyChar);
                        cursorPosition++;
                    }

                    break;
            }

            Updated?.Invoke(GetContextString(sb.ToString(), cursorPosition));
            UpdateClearString(sb.ToString());
        }

        Completed?.Invoke(sb.ToString());
        _history.Add(sb.ToString());
        return sb.ToString();
    }

    public void Clear()
    {
    }

    public string ClearString { get; private set; } = Esc.ClearCurrentLine;

    private void UpdateClearString(string str)
    {
        int height = str.Split('\n').Length;
        StringBuilder sb = new(Esc.ClearCurrentLine);
        for (int i = 0; i < height - 1; i++)
        {
            sb.Append(Environment.NewLine + Esc.ClearCurrentLine);
        }

        if (height > 1)
        {
            sb.Append(Esc.CursorUp(height - 1));
        }

        ClearString = sb.ToString();
    }

    private string GetContextString(string s, int i)
    {
        s += " ";
        string strPrefix = s[..i];
        char ch = s[i];
        string strSuffix = s[(i + 1)..];

        string str =
            strPrefix.Color(TextColor) +
            ch.Color(CursorColor) +
            strSuffix.Color(TextColor);

        Height = str.Split('\n').Length;

        return str;
    }

    private string AllKeyWordsToString(string commonPrefix)
    {
        StringBuilder sb = new();
        List<string> keyWords = _keyWords
            .Where(word => word.StartsWith(commonPrefix))
            .ToList();
        keyWords.Sort();
        int maxLength = keyWords
            .Select(s => s.Length)
            .Max();
        int cntWordInLine = Console.WindowWidth / (maxLength + 5);
        int alignment = Console.WindowWidth / cntWordInLine;

        int cnt = 0;
        foreach (string word in keyWords)
        {
            sb.Append(word.PadRight(alignment));
            if (++cnt == keyWords.Count)
                break;
            if ((cnt % cntWordInLine) == 0)
                sb.Append(Environment.NewLine);
        }

        return sb.ToString();
    }

    public class AutoCompleteInputBuilder
    {
        private readonly AutoCompleteInput _autoCompleteInput;

        internal AutoCompleteInputBuilder(AutoCompleteInput autoCompleteInput) =>
            _autoCompleteInput = autoCompleteInput;

        public AutoCompleteInputBuilder TextColor(EscColor color)
        {
            _autoCompleteInput.TextColor = color;
            return this;
        }
        
        public AutoCompleteInputBuilder CursorColor(EscColor color)
        {
            _autoCompleteInput.CursorColor = color;
            return this;
        }

        public AutoCompleteInput Build() =>
            _autoCompleteInput;
    }
}