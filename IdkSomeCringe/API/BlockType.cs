namespace IdkSomeCringe.API;

[Flags]
public enum BlockType
{
    None = 0,
    Reserved = 1,
    Wall = 2,
    Food = 4,
    Snake = 8,
    BigFood = 16,
    ReservedWall = Reserved | Wall
}
