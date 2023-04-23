using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace IdkSomeCringe.LowLevel;

internal static partial class Externs
{
    [LibraryImport("Kernel32.dll", SetLastError = true)]
    internal static partial SafeFileHandle GetStdHandle(
        [MarshalAs(UnmanagedType.U4)] uint nStdHandles);

    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool WriteConsoleOutput(
        SafeFileHandle hConsoleOutput,
        CharInfo[] lpBuffer,
        Coord dwBufferSize,
        Coord dwBufferCoord,
        ref SmallRect lpAllScreenRect);
}

[StructLayout(LayoutKind.Sequential)]
public struct Coord
{
    public short X;
    public short Y;

    public Coord(short x, short y)
    {
        X = x;
        Y = y;
    }
}

[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
public struct CharInfo
{
    [FieldOffset(0)] public char UnicodeChar;
    [FieldOffset(2)] public short Attributes;

    public CharInfo(char unicode, short attributes)
    {
        UnicodeChar = unicode;
        Attributes = attributes;
    }

    public CharInfo(char unicode, ConsoleColor foreground, ConsoleColor background)
    {
        UnicodeChar = unicode;
        Attributes = (short)((int)foreground | ((int)background << 4));
    }

    public CharInfo(char unicode)
        : this(unicode, ConsoleColor.White, ConsoleColor.Black)
    {
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct SmallRect
{
    public short Left;
    public short Top;
    public short Right;
    public short Bottom;
}
