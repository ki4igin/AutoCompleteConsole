using System.Text;
using MyConsole;
using MyConsole.InputProvider;

MainConsole mc = new();

// Console.Write(Esc.CursorHide);


// var progressBar = new ProgressBar(100);
// for (int i = 0; i <= 100; i++)
// {
//     progressBar.Increment();
//     Thread.Sleep(100);
// }



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
    .Create("Whats?", s => int.TryParse(s, out int _), "55")
    .Build();

ProgressBar testInput = new ProgressBar();
IDisposable testStatus = mc.CreateStatus(testInput);
IDisposable reqStatus = mc.CreateStatus(request);
IDisposable selStatus = mc.CreateStatus(selector);

mc.WriteLine("Привет, мир!");

// testInput.Write(new string('=', 40));
// selector.Updated = s => selStatus.Write(s);
// rd.Completed = s => wr.WriteLine(s.Color(EscColor.ForegroundDarkYellow));
// rd.Updated = s => rdrStatus.Write(s);

int cnt = 0;
Task.Run(() =>
{
    
    while (true)
    {
        string str = $"{cnt++ % 1000}";
        if (cnt % 16 == 0)
        {
            testInput.Report(cnt / 1000.0);
            mc.Write(str);
        }
        else
            mc.Write(str, EscColor.ForegroundRed);
        Thread.Sleep(10);
    }
});

// selector.Run();

while (true)
{
    string cmd = mc.ReadLine();
    if (cmd == "clr")
    {
        cnt = 0;
    }
    if (cmd == "sel")
    {
        // rdrStatus.Redirect(selector);
        selector.Run();
        // rdrStatus.Redirect(rd);
    }

    if (cmd == "req")
    {
        // rdrStatus.Redirect(selector);
        request.ReadLine();
        // rdrStatus.Redirect(rd);
    }
    if (cmd == "quit")
    {
        // rdrStatus.Redirect(selector);
        return;
        // rdrStatus.Redirect(rd);
    }
}
