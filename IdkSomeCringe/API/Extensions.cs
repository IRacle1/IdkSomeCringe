namespace IdkSomeCringe.API;

public static class Extensions
{
    public static bool HasFlagFast(this BlockType blockType, BlockType flag)
    {
        return (blockType & flag) == flag;
    }

    public static bool IsEmpty(this Vector2 vec) =>
        GetBlockType(vec) == BlockType.None;

    public static bool IsWall(this Vector2 vec) =>
        GetBlockType(vec).HasFlagFast(BlockType.Wall);

    public static bool IsFood(this Vector2 vec) =>
        GetBlockType(vec).HasFlagFast(BlockType.Food);

    public static bool IsSnake(this Vector2 vec) =>
        GetBlockType(vec).HasFlagFast(BlockType.Snake);

    public static BlockType GetBlockType(this Vector2 vec)
    {
        BlockType type = BlockType.None;

        if (PrintManager.Foods.Contains(vec))
        {
            type |= BlockType.Food;
        }

        if (vec.X == 0 || vec.X == PrintManager.Width - 1 || 
            vec.Y == 0 || vec.Y == PrintManager.Height - 1)
        {
            type |= BlockType.Wall;
        }

        if (PrintManager.BodyParts.Contains(vec))
        {
            type |= BlockType.Snake;
        }

        return type;
    }
}
