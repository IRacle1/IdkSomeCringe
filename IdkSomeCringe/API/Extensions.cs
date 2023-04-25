namespace IdkSomeCringe.API;

public static class Extensions
{
    public static bool HasFlagFast(this BlockType blockType, BlockType flag)
    {
        return (blockType & flag) == flag;
    }

    public static bool IsCachedBlockType(this BlockType blockType)
    {
        return blockType is BlockType.ReservedWall or BlockType.Wall or BlockType.Reserved;
    }

    public static double MapRange(double value, (double Start, double Stop) range1, (double Start, double Stop) range2)
    {
        return (value - range1.Start) /
            (range1.Stop - range1.Start) *
            (range2.Stop - range2.Start) +
            range2.Start;
    }
}
