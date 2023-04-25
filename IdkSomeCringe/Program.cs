using IdkSomeCringe.API;
using IdkSomeCringe.LowLevel;

namespace IdkSomeCringe;

internal class Program
{
    private static GameSettings MainSettings { get; } = new GameSettings
    {
        BlockInfos = new(new Dictionary<BlockType, CharInfo>()
        {
            { BlockType.None, new(' ') },
            { BlockType.Wall, new('#', ConsoleColor.DarkGray, ConsoleColor.Black) },
            { BlockType.Food, new('*', ConsoleColor.Green, ConsoleColor.Black) },
            { BlockType.ReservedWall, new('#', ConsoleColor.Gray, ConsoleColor.Black) },
        }),
        Width = 90,
        Height = 30,
        Gradient = new()
        {
            ConsoleColor.Cyan,
            ConsoleColor.DarkCyan,
            ConsoleColor.Blue,
            ConsoleColor.DarkBlue,
        }
    };

    static void Main(string[] args)
    {
        int height = 30;
        int width = 90;

        Console.SetWindowSize(width, height);
        Console.SetBufferSize(width + 1, height + 1);

        Console.CursorVisible = false;
        Console.Title = "Snake idk";

        Console.OutputEncoding = System.Text.Encoding.Unicode;

        GameManager gm = new(MainSettings);

        while (true)
        {
            gm.Update();
            Thread.Sleep(1);
        }
    }
}
