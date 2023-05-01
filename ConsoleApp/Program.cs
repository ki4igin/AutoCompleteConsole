using AutoCompleteConsole;
using AutoCompleteConsole.StringProvider;

// Console.Write(Esc.CursorHide);


// var progressBar = new ProgressBar(100);
// for (int i = 0; i <= 100; i++)
// {
//     progressBar.Increment();
//     Thread.Sleep(100);
// }


Selector selector = Acc.CreateSelector(new());
Request request = Acc.CreateRequest(new());
Status status = Acc.CreateStatus();
status.Change(new('-', 10));
ProgressBar testInput = Acc.CreateProgressBar();
ProgressBar testInput2 = Acc.CreateProgressBar();

// IDisposable testStatus = mc.CreateStatus(testInput);
Acc.EnableScreenBufferAlternative();
Acc.WriteLine("Привет, мир!");
// Console.Write("\u001b[2;15r");

// testInput.Write(new string('=', 40));
// selector.Updated = s => selStatus.Write(s);
// rd.Completed = s => wr.WriteLine(s.Color(EscColor.ForegroundDarkYellow));
// rd.Updated = s => rdrStatus.Write(s);

int cnt = 0;
Task.Run(() =>
{
    while (true)
    {
        string str = $"{cnt++ % 2}";
        if (cnt % 16 == 0)
        {
            testInput.Report(cnt / 1000.0);
            testInput2.Report(cnt / 100.0);
            Acc.Write(str);
            // status.Change("");
        }
        else
            Acc.Write(str, EscColor.ForegroundRed);

        Thread.Sleep(100);
    }
});

// selector.Run();

while (true)
{
    string cmd = Acc.ReadLine();
    Acc.WriteLine(Environment.NewLine+cmd, EscColor.ForegroundDarkYellow);
    if (cmd == "clr")
    {
        cnt = 0;
    }
    if (cmd == "movup")
    {
        Console.Write("\u001b[5S");
    }
    if (cmd == "movdown")
    {
        Console.Write("\u001b[5T");
    }

    if (cmd == "sel")
    {
        // rdrStatus.Redirect(selector);
        selector.Run(
            new(
                "Menu",
                new[] {"clear", "start", "stop", "text", "quit"}
            ),
            2
        );
        // rdrStatus.Redirect(rd);
    }

    if (cmd == "req")
    {
        // rdrStatus.Redirect(selector);
       
        // rdrStatus.Redirect(rd);
    }

    if (cmd == "quit")
    {
        // rdrStatus.Redirect(selector);
        return;
        // rdrStatus.Redirect(rd);
    }
}