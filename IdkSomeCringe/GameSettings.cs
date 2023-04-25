using IdkSomeCringe.API;
using System.Collections.ObjectModel;

using IdkSomeCringe.LowLevel;

namespace IdkSomeCringe;

public class GameSettings
{
    public required int Height { get; init; }
    public required int Width { get; init; }

    public Func<List<Vector2>, int, CharInfo> BodyPickerFunction => SecondFunction;

    public required ReadOnlyDictionary<BlockType, CharInfo> BlockInfos { get; init; } = null!;

    public required List<ConsoleColor> Gradient { get; init; } = null!;

    public ConsoleColor MapGradient(double progress)
    {
        return Gradient[(int)Math.Round(Extensions.MapRange(progress, (0, 1), (0, Gradient.Count - 1)), 0)];
    }

    public CharInfo FirstFunction(List<Vector2> snakeBody, int index)
    {
        double prog = index / (double)Math.Clamp(snakeBody.Count - 1, 1, snakeBody.Count);
        return new CharInfo(snakeBody.GetSnakePartChar(index), MapGradient(prog), ConsoleColor.Black);
    }

    public CharInfo SecondFunction(List<Vector2> snakeBody, int index)
    {
        return new CharInfo(snakeBody.GetSnakePartChar(index), Gradient[index % Gradient.Count], ConsoleColor.Black);
    }
}
