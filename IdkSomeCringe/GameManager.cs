using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using IdkSomeCringe.API;

using IdkSomeCringe.LowLevel;

using static System.Reflection.Metadata.BlobBuilder;

namespace IdkSomeCringe
{
    public class GameManager
    {
        public GameSettings Settings = null!;

        private KeyCode[] keyCodes = Enum.GetValues<KeyCode>();

        private Dictionary<Vector2, BlockType> blocks = new();

        private CharInfo[] chars = null!;

        public List<Vector2> BodyParts = new();
        public List<Vector2> Foods = new();

        private KeyCode lastKeyCode = KeyCode.Up;

        public int FramesPerMove = 6;
        private int currentFrames = 0;

        public ReadOnlyDictionary<Vector2, BlockType> ReadOnlyBlocks = null!;

        public int Height => Settings.Height;
        public int Width => Settings.Width;

        public Func<List<Vector2>, int, CharInfo> BodyPickerFunction => Settings.BodyPickerFunction;

        public ReadOnlyDictionary<BlockType, CharInfo> BlockInfos => Settings.BlockInfos;

        public GameManager(GameSettings settings)
        {
            Settings = settings;

            chars = new CharInfo[Height * Width];

            BodyParts.Add(new Vector2(Width / 2, Height / 2));

            ReadOnlyBlocks = blocks.AsReadOnly();

            UpdateChars(true);

            SpawnFood();
        }

        public void UpdateChars(bool init)
        {
            Vector2 vec = new(0, 0);

            for (int i = 0; i < Height; i++)
            {
                vec.Y = i;
                for (int j = 0; j < Width; j++)
                {
                    vec.X = j;

                    BlockType type = blocks[vec] = GetBlockType(vec);

                    if (type.IsCachedBlockType() && !init)
                        continue;
                    
                    if (BlockInfos.TryGetValue(type, out CharInfo info))
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

        public void Update()
        {
            FastConsole.Write(chars);

            ProcessKey();

            if (!ProcessFrameCheck())
                return;

            if (ProcessMovement())
            {
                UpdateChars(false);

                Vector2 vec = new(3, 1);

                string str = "Твои очки: ";

                for (int i = 0; i < str.Length; i++)
                {
                    chars[vec.MapToArray(Width)] = new CharInfo(str[i], ConsoleColor.White, ConsoleColor.Black);
                    vec += Vector2.Right; 
                }

                str = BodyParts.Count.ToString();

                for (int i = 0; i < str.Length; i++)
                {
                    chars[vec.MapToArray(Width)] = new CharInfo(str[i], ConsoleColor.Green, ConsoleColor.Black);
                    vec += Vector2.Right;
                }

                for (int i = BodyParts.Count - 1; i >= 0; i--)
                {
                    chars[BodyParts[i].MapToArray(Width)] = BodyPickerFunction(BodyParts, i);
                }

                if (Foods.Count == 0)
                    SpawnFood();
            }


        }

        public bool ProcessFrameCheck()
        {
            currentFrames++;

            if (currentFrames < FramesPerMove)
            {
                return false;
            }

            currentFrames = 0;

            return true;
        }

        public void SpawnFood()
        {
            var keyValue = ReadOnlyBlocks.Where((keyValue) => keyValue.Value == BlockType.None).OrderBy(_ => Random.Shared.Next()).FirstOrDefault();

            Foods.Add(keyValue.Key);
        }

        public double MapRange(double value, (double Start, double Stop) range1, (double Start, double Stop) range2)
        {
            return (value - range1.Start) /
                (range1.Stop - range1.Start) *
                (range2.Stop - range2.Start) +
                range2.Start;
        }

        public void ProcessKey()
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

        public bool ProcessMovement()
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

            switch (GetBlockType(last))
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

        public void Reset()
        {
            Thread.Sleep(1000);

            BodyParts.Clear();

            Foods.Clear();

            chars = new CharInfo[Height * Width];

            BodyParts.Add(new Vector2(Width / 2, Height / 2));

            UpdateChars(true);

            SpawnFood();

            GC.Collect();
        }

        public BlockType GetBlockType(Vector2 vec)
        {
            BlockType type = BlockType.None;

            if (Foods.Contains(vec))
            {
                type |= BlockType.Food;
            }

            if (vec.X == 0 || vec.X == Width - 1 ||
                vec.Y <= 3 || vec.Y == Height - 1)
            {
                type |= BlockType.Wall;
            }

            if (vec.X >= 0 && vec.X <= Width - 1 && vec.Y < 3)
            {   
                type |= BlockType.Reserved;

                if (vec.X != 0 && vec.X != Width - 1 && vec.Y != 0)
                {
                    type &= ~BlockType.Wall;
                }
            }

            if (BodyParts.Contains(vec))
            {
                type |= BlockType.Snake;
            }

            return type;
        }

        public bool IsEmpty(Vector2 vec) =>
            GetBlockType(vec) == BlockType.None;

        public bool IsWall(Vector2 vec) =>
            GetBlockType(vec).HasFlagFast(BlockType.Wall);

        public bool IsFood(Vector2 vec) =>
            GetBlockType(vec).HasFlagFast(BlockType.Food);

        public bool IsSnake(Vector2 vec) =>
            GetBlockType(vec).HasFlagFast(BlockType.Snake);
    }
}
