// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using AutoCompleteConsole;

NativeTerminal.EnableVirtualTerminalProcessing();

Console.WriteLine("Hello, World!");

while (true)
{
     // var key = Console.ReadKey(true);
     // switch (key.Key)
     // {
     //      case ConsoleKey.LeftArrow:
     //           Console.Write(Esc.CursorLeft());
     //           break;
     //      case ConsoleKey.RightArrow:
     //           Console.Write(Esc.CursorRight());
     //           break;
     //      case ConsoleKey.Enter:
     //           Console.WriteLine("Exit...");
     //           return;
     //      default:
     //           Console.Write(key.KeyChar);
     //           break;
     // }
     int pos = int.Parse(Console.ReadLine());
     Console.Write(Esc.CursorPositionLeft(pos));
     Debug.WriteLine(Console.GetCursorPosition());
}
