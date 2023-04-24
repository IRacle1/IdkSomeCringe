namespace IdkSomeCringe.API;

[Flags]
public enum VectorLocation
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8,
    UpRight = Up | Right,
    DownRight = Down | Right,
    DownLeft = Down | Left,
    UpLeft = Up | Left,
}

public struct Vector2 : IEquatable<Vector2>
{
    public Vector2(int x, int y)
    {
        (X, Y) = (x, y);
    }

    public Vector2(int value)
        : this(value, value)
    {
    }

    public int X { get; set; }
    public int Y { get; set; }

    public int SqrMagnitude => X * X + Y * Y;

    public readonly int MapToArray(int width) =>
        Y * width + X;

    public readonly bool Equals(Vector2 other) =>
        X == other.X && Y == other.Y;

    public static Vector2 Zero { get; } = new Vector2(0, 0);
    public static Vector2 Up { get; } = new Vector2(0, -1);
    public static Vector2 Down { get; } = new Vector2(0, 1);
    public static Vector2 Right { get; } = new Vector2(1, 0);
    public static Vector2 Left { get; } = new Vector2(-1, 0);

    public static Vector2 operator -(Vector2 vector) =>
        new(-vector.X, -vector.Y);

    public static Vector2 operator +(Vector2 left, Vector2 right) =>
        new(left.X + right.X, left.Y + right.Y);

    public static Vector2 operator -(Vector2 left, Vector2 right) =>
        new(left.X - right.X, left.Y - right.Y);

    public static Vector2 operator *(Vector2 left, Vector2 right) =>
        new(left.X * right.X, left.Y * right.Y);

    public static Vector2 operator /(Vector2 left, Vector2 right) =>
        new(left.X / right.X, left.Y / right.Y);

    public static implicit operator Vector2(int value) =>
        new(value);

    public static float Dot(Vector2 left, Vector2 right) =>
        left.X * right.X + left.Y * right.Y;

    public static Vector2 GetVectorByVectorLocation(VectorLocation vectorLocation)
    {
        switch (vectorLocation)
        {
            case VectorLocation.Left:
                return Left;
            case VectorLocation.Right:
                return Right;
            case VectorLocation.Down:
                return Down;
            case VectorLocation.Up:
                return Up;
            case VectorLocation.DownLeft:
                return Left + Down;
            case VectorLocation.DownRight:
                return Down + Right;
            case VectorLocation.UpLeft:
                return Left + Up;
            case VectorLocation.UpRight:
                return Up + Right;
        }

        return Zero;
    }

    public VectorLocation GetLocation()
    {
        VectorLocation location = VectorLocation.None;

        if (X == -1)
            location |= VectorLocation.Left;
        if (Y == -1)
            location |= VectorLocation.Up;
        if (X == 1)
            location |= VectorLocation.Right;
        if (Y == 1)
            location |= VectorLocation.Down;

        return location;
    }
}
