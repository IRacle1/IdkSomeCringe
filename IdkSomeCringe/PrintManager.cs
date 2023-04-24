using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using IdkSomeCringe.API;

using IdkSomeCringe.LowLevel;

using static System.Reflection.Metadata.BlobBuilder;

namespace IdkSomeCringe
{
    public static class PrintManager
    {
        private static KeyCode[] keyCodes = Enum.GetValues<KeyCode>();

        private static Dictionary<Vector2, BlockType> cachedBlocks = new();

        private static CharInfo[] chars = null!;

        public static Func<int, int, CharInfo> BodyPickerFunction = null!;

        public static int Height;
        public static int Width;

        public static List<Vector2> BodyParts = new();
        public static List<Vector2> Foods = new();

        private static KeyCode lastKeyCode = KeyCode.Up;

        public static int FramesPerMove = 6;
        private static int currentFrames = 0;

        public static ReadOnlyDictionary<Vector2, BlockType> Blocks = null!;

        public static ReadOnlyDictionary<BlockType, CharInfo> BlockInfos = new(new Dictionary<BlockType, CharInfo>()
        {
            { BlockType.None, new(' ') },
            { BlockType.Wall, new('#', ConsoleColor.DarkGray, ConsoleColor.Black) },
            { BlockType.Food, new('*', ConsoleColor.Green, ConsoleColor.Black) },
            { BlockType.ReservedWall, new('#', ConsoleColor.Gray, ConsoleColor.Black) },
        });

        public static ConsoleColor[] Gradient = new[]
        {
            ConsoleColor.Cyan,
            ConsoleColor.DarkCyan,
            ConsoleColor.Blue,
            ConsoleColor.DarkBlue,
        };

        public static void Init(int width, int height, Func<int, int, CharInfo> bodyPickerFunction)
        {
            Height = height;
            Width = width;

            chars = new CharInfo[Height * Width];

            BodyParts.Add(new Vector2(Width / 2, Height / 2));

            Blocks = cachedBlocks.AsReadOnly();

            BodyPickerFunction = bodyPickerFunction;

            Vector2 vec = new(0, 0);

            for (int i = 0; i < Height; i++)
            {
                vec.Y = i;
                for (int j = 0; j < Width; j++)
                {
                    vec.X = j;
                    cachedBlocks[vec] = vec.GetBlockType();
                }
            }

            SpawnFood();
        }

        public static void Draw()
        {
            Vector2 vec = new(0, 0);

            for (int i = 0; i < Height; i++)
            {
                vec.Y = i;
                for (int j = 0; j < Width; j++)
                {
                    vec.X = j;

                    cachedBlocks[vec] = vec.GetBlockType();
                    
                    if (BlockInfos.TryGetValue(cachedBlocks[vec], out CharInfo info))
                    {
                        chars[vec.MapToArray(Width)] = info;
                    }
                    else
                    {
                        chars[vec.MapToArray(Width)] = BlockInfos[BlockType.None];
                    }
                }
            }
        }

        public static void Update()
        {
            FastConsole.Write(chars);

            ProcessKey();

            if (!ProcessFrameCheck())
                return;

            if (ProcessMovement())
            {
                Draw();

                Vector2 vec = new(3, 1);

                string str = "Твои очки: " + BodyParts.Count.ToString();

                for (int i = 0; i < str.Length; i++)
                {
                    chars[vec.MapToArray(Width)] = new CharInfo(str[i], ConsoleColor.White, ConsoleColor.Black);
                    vec += Vector2.Right; 
                }

                for (int i = BodyParts.Count - 1; i >= 0; i--)
                {
                    chars[BodyParts[i].MapToArray(Width)] = BodyPickerFunction(i, BodyParts.Count);
                }

                if (Foods.Count == 0)
                    SpawnFood();
            }


        }

        public static char GetSnakePartChar(int index)
        {
            if (index == 0)
            {
                return 'ȩ';
            }

            VectorLocation location;

            if (index == BodyParts.Count - 1)
            {
                location = (BodyParts[^2] - BodyParts[^1]).GetLocation();
            }
            else
            {
                VectorLocation first = (BodyParts[index - 1] - BodyParts[index]).GetLocation();
                VectorLocation second = (BodyParts[index + 1] - BodyParts[index]).GetLocation();

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

        public static bool ProcessFrameCheck()
        {
            currentFrames++;

            if (currentFrames < FramesPerMove)
            {
                return false;
            }

            currentFrames = 0;

            return true;
        }

        public static void SpawnFood()
        {
            var keyValue = Blocks.Where((keyValue) => keyValue.Value == BlockType.None).OrderBy(_ => Random.Shared.Next()).FirstOrDefault();

            Foods.Add(keyValue.Key);
        }

        public static ConsoleColor ProcessGradient(double progress)
        {
            return Gradient[(int)Math.Round(MapRange(progress, (0, 1), (0, Gradient.Length - 1)), 0)];
        }

        public static double MapRange(double value, (double Start, double Stop) range1, (double Start, double Stop) range2)
        {
            return (value - range1.Start) /
                (range1.Stop - range1.Start) *
                (range2.Stop - range2.Start) +
                range2.Start;
        }

        public static void ProcessKey()
        {
            foreach (KeyCode code in keyCodes)
            {
                if (NativeKeyboard.IsKeyDown(code))
                {
                    lastKeyCode = code;
                    break;
                }
            }
        }

        public static bool ProcessMovement()
        {
            Vector2 last = BodyParts[0];

            switch (lastKeyCode)
            {
                case KeyCode.Left:
                    last.X--;
                    break;
                case KeyCode.Up:
                    last.Y--;
                    break;
                case KeyCode.Right:
                    last.X++;
                    break;
                case KeyCode.Down:
                    last.Y++;
                    break;
            }

            switch (last.GetBlockType())
            {
                case BlockType.Wall or BlockType.Snake:
                    Reset();
                    return false;
                case BlockType.Food:
                    BodyParts.Add(BodyParts[^1] - (BodyParts[^1] - last));
                    Foods.Remove(last);
                    break;
                case BlockType.BigFood:
                    // TODO
                    break;
                case BlockType.None:
                    break;
            }

            for (int i = 0; i < BodyParts.Count; i++)
            {
                (BodyParts[i], last) = (last, BodyParts[i]);
            }

            return true;
        }

        public static void Reset()
        {
            Thread.Sleep(1000);
            BodyParts.Clear();
            Foods.Clear();
            Init(Width, Height, BodyPickerFunction);
            GC.Collect();
        }

        public static int MaptoArray(int y, int width, int x)
        {
            return y * width + x;
        }
    }
}
