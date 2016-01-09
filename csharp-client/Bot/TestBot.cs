using System;
using System.Collections.Generic;
using System.Linq;
using Coveo.Bot;
using Microsoft.Win32;

namespace CoveoBlitz.RandomBot
{
    public class TestBot : ISimpleBot
    {
        private readonly Random random = new Random();

        public int CostToTavern { get; set; }
        public int CostToMine { get; set; }
        public int Life { get; set; }
        public int Gold { get; set; }

        public List<Pos> Mines = new List<Pos>();
        public List<Pos> Tavernes = new List<Pos>();

        private bool setup = false;

        public void Setup()
        {

        }

        public void Shutdown()
        {
        }

        public string Move(GameState state)
        {

            // Update Info
            Life = state.myHero.life;
            Gold = state.myHero.gold;
            var pos = state.myHero.pos;
            var board = state.board;

            // Initial setup
            if (!setup)
            {
                // Calculate cost to nearest tavern
                // Calculate cost to nearest mine
                GetImportantPos(state.board);



                setup = true;
            }

            Console.WriteLine(pos.x + ", "+ pos.y);

            Pos north = new Pos {x = pos.x-1, y = pos.y};
            Pos south = new Pos {x = pos.x+1, y = pos.y};
            Pos east = new Pos {x = pos.x, y = pos.y+1};
            Pos west = new Pos {x = pos.x, y = pos.y-1};

            Console.WriteLine(board.At(north).ToString() + ", "+ board.At(south).ToString() + ", " + board.At(east).ToString() + ", " + board.At(west).ToString());

            if (Life > 25)
            {

                // If adjacent mine present
                if (MineToClaim(board.At(north), state.myHero.id))
                    return Direction.North;
                if (MineToClaim(board.At(south), state.myHero.id))
                    return Direction.South;
                if (MineToClaim(board.At(west), state.myHero.id))
                    return Direction.West;
                if (MineToClaim(board.At(east), state.myHero.id))
                    return Direction.East;
            }

            if (Gold > 1 && Life < 65)
            {
                //Check for healing
                if (board.At(north) == Tile.TAVERN)
                    return Direction.North;
                if (board.At(south) == Tile.TAVERN)
                    return Direction.South;
                if (board.At(west) == Tile.TAVERN)
                    return Direction.West;
                if (board.At(east) == Tile.TAVERN)
                    return Direction.East;
            }

            //// Check if life enough to subsist
            //if (Life < CostToTavern + Constant.LifeDrainOnHit)
            //{
            //    // Pathfind to tavern

            //}
            //else
            //{
            //    // Pathfind to Mine
            //}

           
            // Target player with maximum mines
            var MaxMinePlayer = new Pos();
            var MaxMine = 0;
            var enemyLife = 0;
            foreach (var hero in state.heroes)
            {
                if (hero.gold > MaxMine)
                {
                    if (hero.pos != state.myHero.pos)
                    {
                        MaxMine = hero.mineCount;
                        MaxMinePlayer = hero.pos;
                        enemyLife = hero.life;
                    }
                }
            }

            if(MaxMine > 0 && enemyLife < state.myHero.life)
                return CalculatePath(state, MaxMinePlayer);

            //Check si assez vie, alors pathfind to
            string direction = string.Empty;
            int count = 0;
            do
            {
                var newRand = random.Next(0, 4);
                Console.WriteLine(newRand);
                switch (newRand)
                {
                    case 0:
                        direction = Direction.East;
                        break;

                    case 1:
                        direction = Direction.West;
                        break;

                    case 2:
                        direction = Direction.North;
                        break;

                    case 3:
                        direction = Direction.South;
                        break;
                }
                count++;
            } while (BadChoice(direction, pos, board) && count < 4);
            if (count < 4)
                return direction;


            Console.Write("No choices");
            return Direction.Stay;
        }
        
        private bool BadChoice(string direction, Pos pos, Tile[][] board)
        {
            Pos movPos = pos;
            switch (direction)
            {
                case Direction.South:
                    movPos.x++;
                    break;
                case Direction.North:
                    movPos.x --;
                    break;
                case Direction.West:
                    movPos.y--;
                    break;
                case Direction.East:
                    movPos.y++;
                    break;

            }
            Tile movTile = board.At(movPos);
            return movTile == Tile.SPIKES || movTile == Tile.IMPASSABLE_WOOD || (movTile <= Tile.GOLD_MINE_4 && movTile >=Tile.GOLD_MINE_NEUTRAL) || movTile == Tile.TAVERN;
        }
    



        public bool OurMine(Tile tile, int hero)
        {
            bool ours = false;

            switch (hero)
            {
                case 1:
                    ours = tile == Tile.GOLD_MINE_1;
                    break;
                case 2:
                    ours = tile == Tile.GOLD_MINE_2;
                    break;
                case 3:
                    ours = tile == Tile.GOLD_MINE_3;
                    break;
                case 4:
                    ours = tile == Tile.GOLD_MINE_4;
                    break;
            }

            return ours;

        }

        public bool MineToClaim(Tile tile, int hero)
        {
            return (tile == Tile.GOLD_MINE_1 || tile == Tile.GOLD_MINE_2 || tile == Tile.GOLD_MINE_3 ||
                    tile == Tile.GOLD_MINE_4 || tile == Tile.GOLD_MINE_NEUTRAL) && !OurMine(tile, hero);
        }

        public void GetImportantPos(Tile[][] board)
        {
            for (var i = 0; i < board.Length; i++)
            {
                for (var j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] >= Tile.GOLD_MINE_NEUTRAL && board[i][j] <= Tile.GOLD_MINE_4)
                    {
                        Mines.Add(new Pos() {x = i, y =j});
                    }else if (board[i][j] == Tile.TAVERN)
                    {
                        Tavernes.Add(new Pos() {x = i, y = j});
                    }
                }
            }
        }



        private string CalculatePath(GameState state, Pos goal)
        {
            Pos start = state.myHero.pos;

            Console.WriteLine("Begining calculating path from ({0},{1}) to ({2},{3})", start.x, start.y, goal.x, goal.y);

            try
            {
                List<Pos> visited = new List<Pos>();
                List<PathCoord> availableTiles = new List<PathCoord>();

                var first = createPathCoord(start, null, state, goal, null);
                availableTiles.Add(first);

                while (availableTiles.Any())
                {
                    // Sort availableTiles
                    availableTiles.Sort((f1, f2) => f1.heuristic.CompareTo(f2.heuristic));

                    var currentVisited = availableTiles.First();
                    availableTiles.Remove(currentVisited);

                    if (visited.Any(x => x.x == currentVisited.current.x && x.y == currentVisited.current.y))
                    {
                        continue;
                    }

                    // Console.WriteLine("Visiting ({0},{1}) with heuristic {2}", currentVisited.current.x, currentVisited.current.y, currentVisited.heuristic);

                    visited.Add(currentVisited.current);

                    if (getDistance(currentVisited.current, goal) == 0)
                    {
                        string direction = Restitute(start, currentVisited);
                        Console.WriteLine("Path found! Going {0}", direction);
                        return direction;
                    }
                    else
                    {
                        var spreadFrom = getAvailableCoords(currentVisited, state, goal);
                        availableTiles.AddRange(spreadFrom);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("No path found!");
            return Direction.Stay;
        }

        private string Restitute(Pos start, PathCoord foundPath)
        {
            while (foundPath != null)
            {
                int distance = getDistance(start, foundPath.current);
                if (distance == 1)
                {
                    return foundPath.previousDirection;
                }
                foundPath = foundPath.previous;
            }

            Console.WriteLine("Error with restitute");
            return Direction.Stay;
        }


        private List<PathCoord> getAvailableCoords(PathCoord current, GameState state, Pos goal)
        {
            PathCoord east = createPathCoord(new Pos() { x = current.current.x, y = current.current.y + 1 }, current, state,
                goal, Direction.East);
            PathCoord west = createPathCoord(new Pos() { x = current.current.x, y = current.current.y - 1 }, current, state,
                goal, Direction.West);
            PathCoord north = createPathCoord(new Pos() { x = current.current.x - 1, y = current.current.y }, current, state,
                goal, Direction.North);
            PathCoord south = createPathCoord(new Pos() { x = current.current.x + 1, y = current.current.y }, current, state,
                goal, Direction.South);

            var rc = new List<PathCoord>();
            if (east != null)
            {
                rc.Add(east);
            }
            if (west != null)
            {
                rc.Add(west);
            }
            if (north != null)
            {
                rc.Add(north);
            }
            if (south != null)
            {
                rc.Add(south);
            }
            return rc;
        }

        private bool isValid(GameState state, Pos coordToValidate)
        {
            int size = state.board.GetLength(0);

            if (coordToValidate.x < 0 || coordToValidate.x >= size ||
                coordToValidate.y < 0 || coordToValidate.y >= size)
            {
                return false;
            }

            return true;
        }

        private PathCoord createPathCoord(Pos current, PathCoord previous, GameState state, Pos goal, string previousDirection)
        {
            try
            {
                if (!isValid(state, current))
                {
                    return null;
                }

                PathCoord pc = new PathCoord();

                var currentTile = state.board.At(current);
                if (( currentTile!= Tile.FREE || currentTile != Tile.SPIKES)  && areEqual(goal, current))
                {
                    return null;
                }

                if (previous != null)
                {
                    pc.weight = previous.weight; // to modify
                }

                pc.weight += getCost(state.board[current.y][current.x]);
                pc.current = current;
                pc.previous = previous;
                pc.previousDirection = previousDirection;

                pc.heuristic = pc.weight + getDistance(current, goal);
                return pc;

            }
            catch (Exception e)
            {
                Console.WriteLine("Error with bound");
                throw e;
            }
        }

        private int getCost(Tile tile)
        {
            int cost;
            switch (tile)
            {
                case Tile.SPIKES:
                    cost = 5;
                    break;
                default:
                    cost = 1;
                    break;
            }
            return cost;
        }

        private int getDistance(Pos a, Pos b)
        {
            return Math.Abs(b.x - a.x) + Math.Abs(b.y - a.y);
        }

        private bool areEqual(Pos a, Pos b)
        {
            return a.x == b.x && a.y == b.y;
        }
    }
}