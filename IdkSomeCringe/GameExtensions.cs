using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IdkSomeCringe.API;
using IdkSomeCringe.LowLevel;

namespace IdkSomeCringe;

public static class GameExtensions
{
    public static char GetSnakePartChar(this List<Vector2> snakeBody, int index)
    {
        if (index == 0)
        {
            return 'ȩ';
        }

        VectorLocation location;

        if (index == snakeBody.Count - 1)
        {
            location = (snakeBody[^2] - snakeBody[^1]).GetLocation();
        }
        else
        {
            VectorLocation first = (snakeBody[index - 1] - snakeBody[index]).GetLocation();
            VectorLocation second = (snakeBody[index + 1] - snakeBody[index]).GetLocation();

            location = first | second;
        }

        switch (location)
        {
            case VectorLocation.UpLeft:
                return '╝';
            case VectorLocation.UpRight:
                return '╚';
            case VectorLocation.DownRight:
                return '╔';
            case VectorLocation.DownLeft:
                return '╗';
            case { } when location.HasFlag(VectorLocation.Up) || location.HasFlag(VectorLocation.Down):
                return '║';
            case { } when location.HasFlag(VectorLocation.Left) || location.HasFlag(VectorLocation.Right):
                return '═';
        }

        return '?';
    }

    public static int MaptoArray(int y, int width, int x)
    {
        return y * width + x;
    }
}
