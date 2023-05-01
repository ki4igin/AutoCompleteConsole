using System.Runtime.InteropServices;

namespace AutoCompleteConsole;

public static class NativeTerminal
{
    [Flags]
    private enum OutputModeFlags
    {
        // EnableProcessedOutput = 0x01,
        // EnableWrapAtEolOutput = 0x02,
        EnableVirtualTerminalProcessing = 0x04
    }
    
    private const int StdOutHandle = -11;

    [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true,
        CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int GetStdHandle(int nStdHandle);
    
    [DllImport("kernel32.dll", EntryPoint = "GetConsoleMode", SetLastError = true,
        CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int GetConsoleMode(int hConsoleHandle, ref int dwMode);
    
    [DllImport("kernel32.dll", EntryPoint = "SetConsoleMode", SetLastError = true,
        CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int SetConsoleMode(int hConsoleHandle, int dwMode);

    public static int EnableVirtualTerminalProcessing()
    {
        OperatingSystem system = Environment.OSVersion;

        if (system.Platform == PlatformID.Win32NT)
        {
            int consoleHandle = GetStdHandle(StdOutHandle);
            int dwMode = 0;
            GetConsoleMode(consoleHandle, ref dwMode);
            if ((dwMode & (int)OutputModeFlags.EnableVirtualTerminalProcessing) !=
                (int)OutputModeFlags.EnableVirtualTerminalProcessing)
            {
                return SetConsoleMode(consoleHandle, dwMode | (int)OutputModeFlags.EnableVirtualTerminalProcessing);
            }
        }

        return 0;
    }
}