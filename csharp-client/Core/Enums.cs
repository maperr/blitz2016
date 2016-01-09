namespace CoveoBlitz
{
    public enum Tile
    {
        IMPASSABLE_WOOD,
        FREE,
        SPIKES,
        HERO_1,
        HERO_2,
        HERO_3,
        HERO_4,
        TAVERN,
        GOLD_MINE_NEUTRAL,
        GOLD_MINE_1,
        GOLD_MINE_2,
        GOLD_MINE_3,
        GOLD_MINE_4
    }

    public class Direction
    {
        public const string Stay = "Stay";
        public const string North = "North";
        public const string East = "East";
        public const string South = "South";
        public const string West = "West";
    }

    public class Constant
    {
        public const int LifeDrainOnHit = 25;
    }

    public static class extensions
    {
        public static Tile At(this Tile[][] tiles, Pos pos)
        {
            if (pos.x < 0 || pos.y < 0 || pos.x > tiles.Length || pos.y > tiles[0].Length)
                return Tile.IMPASSABLE_WOOD;
            return tiles[pos.y][pos.x];
        }
    }
}