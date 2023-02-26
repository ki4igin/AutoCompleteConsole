using System.Text;
using MyConsole;
using MyConsole.InputProvider;

NativeTerminal.EnableVirtualTerminalProcessing();

Console.Write(Esc.Clear);
// Console.Write(Esc.CursorHide);


AutoCompleteInput rd = AutoCompleteInput
    .Create(new[]
        {
            "clear", "start", "stop", "text", "quit",
            "clear", "start", "stop", "text", "quit",
            "clear", "start", "stop", "text", "quit",
            "clear", "start", "stop", "text", "quit",
            "clear", "start", "stop", "text", "quit",
            "clear", "start", "stop", "text", "quit",
            "clear", "start", "stop", "text", "quit"
        }
    )
    .TextColor(EscColor.ForegroundYellow)
    .Build();
Writer wr = new();
Status rdrStatus = new(wr, rd, 3, EscColor.ForegroundDarkBlue);
Status testStatus = new(wr, new TestInput(), 2, EscColor.ForegroundDarkBlue);
Selector selector = Selector
    .Create(
        "Menu",
        new[] { "clear", "start", "stop", "text", "quit" },
        2)
    .BaseColor(EscColor.ForegroundYellow)
    .ItemsColor(EscColor.ForegroundYellow)
    .SelectColor(EscColor.BackgroundDarkBlue)
    .SubTitle("asdfasdf")
    .SubTitleColor(EscColor.ForegroundGreen)
    .TitleColor(EscColor.ForegroundDarkGreen)
    .SelectColor(EscColor.Underline)
    .Comment("Press Esc")
    .CommentColor(EscColor.ForegroundDarkWhite)
    .Build();
Status selStatus = new(wr, selector, 3, EscColor.ForegroundDarkBlue);

Request request = Request
    .Create("Whats?", s => int.TryParse(s, out int res), "55")
    .Build();
Status reqStatus = new(wr, request, 3, EscColor.ForegroundDarkBlue);

wr.NewLineSuffixString = UpdateStatus;

wr.WriteLine("Привет, мир!");

testStatus.Write("==========================================");


// selector.Updated = s => selStatus.Write(s);

// rd.Completed = s => wr.WriteLine(s.Color(EscColor.ForegroundDarkYellow));
// rd.Updated = s => rdrStatus.Write(s);


Task.Run(() =>
{
    int cnt = 0;
    while (true)
    {
        string str = $"{cnt++ % 1000}";
        if (cnt % 16 == 0)
            wr.Write(str);
        else
            wr.Write(str, EscColor.ForegroundRed);
        Thread.Sleep(10);
    }
});

// selector.Run();

while (true)
{
    string cmd = rd.ReadLine();
    if (cmd == "sel")
    {
        // rdrStatus.Redirect(selector);
        selector.Run();
        // rdrStatus.Redirect(rd);
    }

    if (cmd == "req")
    {
        // rdrStatus.Redirect(selector);
        request.Run();
        // rdrStatus.Redirect(rd);
    }
}

string UpdateStatus() =>
    reqStatus.GetUpdateString() +
    rdrStatus.GetUpdateString() +
    selStatus.GetUpdateString() +
    testStatus.GetUpdateString();

// {
//     return
//         Esc.CursorDown(2) +
//         Reader.ClearString +
//         Environment.NewLine +
//         status.Color(EscColor.ForegroundDarkBlue) +
//         Esc.CursorUp(2 + Reader.Height);
// }