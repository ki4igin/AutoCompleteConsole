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

ProgressBar testInput = new ProgressBar(100);
Status testStatus = wr.CreateStatus(2, testInput);
Status reqStatus = wr.CreateStatus(3, request);
Status rdrStatus = wr.CreateStatus(3, rd);
Status selStatus = wr.CreateStatus(3, selector);

wr.WriteLine("Привет, мир!");

// testInput.Write(new string('=', 40));

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
        {
            testInput.Increment();
            wr.Write(str);
        }
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
