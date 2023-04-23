namespace IdkSomeCringe.API;

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

    public int MapToArray(int width) =>
        Y * width + X;

    public bool Equals(Vector2 other) =>
        X == other.X && Y == other.Y;

    public static Vector2 Up { get; } = new Vector2(0, 1);
    public static Vector2 Down { get; } = new Vector2(0, -1);
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
}                             
