using Microsoft.Win32.SafeHandles;

namespace IdkSomeCringe.LowLevel;

public static class FastConsole
{
    private static SafeFileHandle? consoleOutHandle;

    private static Coord cachedZeroCoord = new(0, 0);

    private static short consoleHeight = (short)Console.WindowHeight;
    private static short consoleWidth = (short)Console.WindowWidth;

    private static SmallRect allScreenRect =
        new() { Left = 0, Top = 0, Right = consoleWidth, Bottom = consoleHeight };

    public static ref readonly SmallRect AllScreenRect => ref allScreenRect;

    private static Coord BufferSize =
        new(consoleWidth, consoleHeight);

    public static SafeFileHandle ConsoleOutHandle  
        => consoleOutHandle ??= Externs.GetStdHandle(uint.MaxValue - 11u);

    public static void Write(char[] text, ConsoleColor fg, ConsoleColor bg)
    {
        CharInfo[] buffer = new CharInfo[consoleWidth * consoleHeight];

        for (int i = 0; i < text.Length; i++)
        {
            // Set character
            buffer[i].UnicodeChar = text[i];

            // Set color
            buffer[i].Attributes = (short)((int)fg | ((int)bg << 4));
        }

        Write(buffer);
    }

    public static void Write(CharInfo[] buffer)
    {
        SmallRect rect = AllScreenRect;
        Externs.WriteConsoleOutput(ConsoleOutHandle, buffer, BufferSize, cachedZeroCoord, ref rect);
    }
}
