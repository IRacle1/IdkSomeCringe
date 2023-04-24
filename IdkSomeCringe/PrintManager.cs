using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using IdkSomeCringe.API;

using IdkSomeCringe.LowLevel;

using static System.Reflection.Metadata.BlobBuilder;

namespace IdkSomeCringe
{
    public class PrintManager
    {
        private static KeyCode[] keyCodes = null!;

        private static Dictionary<Vector2, BlockType> cachedBlocks = new();

        public static CharInfo[] Chars = null!;

        public static int Height;
        public static int Width;

        public static List<Vector2> BodyParts = new();
        public static List<Vector2> Foods = new();

        public static KeyCode LastKeyCode = KeyCode.Up;

        public static int FramesPerMove = 4;
        private static int currentFrames = 0;

        public static ReadOnlyDictionary<Vector2, BlockType> Blocks = null!;

        public static ReadOnlyDictionary<BlockType, CharInfo> BlockInfos = new(new Dictionary<BlockType, CharInfo>()
        {
            { BlockType.None, new(' ') },
            { BlockType.Wall, new('#', ConsoleColor.DarkGray, ConsoleColor.Black) },
            { BlockType.Food, new('*', ConsoleColor.Green, ConsoleColor.Black) },
        });

        private static ConsoleColor[] Gradient = new[]
        {
            ConsoleColor.Cyan,
            ConsoleColor.DarkCyan,
            ConsoleColor.Blue,
            ConsoleColor.DarkBlue,
        };

        public static void Init(int width, int height)
        {
            Height = height;
            Width = width;
            Chars = new CharInfo[Height * Width];

            BodyParts.Add(new Vector2(Width / 2, Height / 2));

            Blocks = cachedBlocks.AsReadOnly();

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
                        Chars[vec.MapToArray(Width)] = info;
                    }
                    else
                    {
                        Chars[vec.MapToArray(Width)] = BlockInfos[BlockType.None];
                    }
                }
            }
        }

        public static void Update()
        {
            ProcessKey();

            if (!ProcessFrameCheck())
                return;

            if (ProcessMovement())
            {
                Draw();

                for (int i = BodyParts.Count - 1; i >= 0; i--)
                {
                    double prog = i / Math.Clamp((double)BodyParts.Count - 1, 1, BodyParts.Count);
                    Chars[BodyParts[i].MapToArray(Width)] = new CharInfo('&', ProcessGradient(prog), ConsoleColor.Black);
                }

                if (Foods.Count == 0)
                    SpawnFood();
            }
        }

        public char GetSnakePartChar(int index)
        {
            if (index == 0)
            {
                return '&';
            }
            VectorLocation location = VectorLocation.None;
            for (int i = 1; i < BodyParts.Count - 2; i++){
                Vector2 first = BodyParts[i] - BodyParts[i - 1];
                location = Vector2.GetVectorLocationByVector(first);
            }
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
            foreach (KeyCode code in keyCodes ??= Enum.GetValues<KeyCode>())
            {
                if (NativeKeyboard.IsKeyDown(code))
                {
                    LastKeyCode = code;
                    break;
                }
            }
        }

        public static bool ProcessMovement()
        {
            Vector2 last = BodyParts[0];

            switch (LastKeyCode)
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
                    // BodyParts.Add(BodyParts[^1] - (BodyParts[^1] - last));
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
            Init(Width, Height);
            GC.Collect();
        }

        public static int MaptoArray(int y, int width, int x)
        {
            return y * width + x;
        }
    }
}
