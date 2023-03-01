using System.Text;
using MyConsole;
using MyConsole.InputProvider;

NativeTerminal.EnableVirtualTerminalProcessing();

Console.Write(Esc.Clear);
// Console.Write(Esc.CursorHide);


// var progressBar = new ProgressBar(100);
// for (int i = 0; i <= 100; i++)
// {
//     progressBar.Increment();
//     Thread.Sleep(100);
// }



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

Request request = Request
    .Create("Whats?", s => int.TryParse(s, out int res), "55")
    .Build();

Status testStatus = wr.CreateStatus(2, new TestInput(), EscColor.ForegroundDarkBlue);
Status reqStatus = wr.CreateStatus(3, request, EscColor.ForegroundDarkBlue);
Status rdrStatus = wr.CreateStatus(3, rd, EscColor.ForegroundDarkBlue);
Status selStatus = wr.CreateStatus(3, selector, EscColor.ForegroundDarkBlue);

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
        testStatus.Dispose();
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
