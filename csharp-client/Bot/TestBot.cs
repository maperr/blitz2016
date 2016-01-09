using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Coveo.Bot;
using Coveo.StateMachine;
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
        public int MyHeroId { get; set; }
        public List<Pos> Mines = new List<Pos>();
        public List<Pos> Tavernes = new List<Pos>();

        private bool setup = false;

        public void Setup()
        {

        }

        public void Shutdown()
        {
        }


        // Exemple de state machine

        private IState currentState = new CaptureMine();

        private string proofOnconcept(GameState state)
        {
            currentState = currentState.CalculateNextState(state, this);
            var nextGoal = currentState.GetGoal(state, this);
            return CalculatePath(state, nextGoal).Item1;
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
                GetImportantPos(state.board);
                MyHeroId = state.myHero.id;
                setup = true;
            }

            //Console.WriteLine(pos.x + ", "+ pos.y);

            Pos north = new Pos {x = pos.x-1, y = pos.y};
            Pos south = new Pos {x = pos.x+1, y = pos.y};
            Pos east = new Pos {x = pos.x, y = pos.y+1};
            Pos west = new Pos {x = pos.x, y = pos.y-1};

            //Console.WriteLine(board.At(north).ToString() + ", "+ board.At(south).ToString() + ", " + board.At(east).ToString() + ", " + board.At(west).ToString());

            if (Life > 25)
            {

                // If adjacent mine present
                if (MineToClaim(board.At(north)))
                    return Direction.North;
                if (MineToClaim(board.At(south)))
                    return Direction.South;
                if (MineToClaim(board.At(west)))
                    return Direction.West;
                if (MineToClaim(board.At(east)))
                    return Direction.East;
            }

            if (Gold > 1 && Life <= 75)
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


            return proofOnconcept(state);
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

        public bool MineToClaim(Tile tile)
        {
            return (tile == Tile.GOLD_MINE_1 || tile == Tile.GOLD_MINE_2 || tile == Tile.GOLD_MINE_3 ||
                    tile == Tile.GOLD_MINE_4 || tile == Tile.GOLD_MINE_NEUTRAL) && !OurMine(tile, MyHeroId);
        }

        public void GetImportantPos(Tile[][] board)
        {
            for (var i = 0; i < board.Length; i++)
            {
                for (var j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] >= Tile.GOLD_MINE_NEUTRAL && board[i][j] <= Tile.GOLD_MINE_4)
                    {
                        Mines.Add(new Pos() {x = j, y =i});
                    }else if (board[i][j] == Tile.TAVERN)
                    {
                        Tavernes.Add(new Pos() {x = j, y = i});
                    }
                }
            }
        }


        public Pos GetClosestMine(Pos pos, Tile[][] board)
        {
            Pos meilleurePos = new Pos();
            int meilleureDist = int.MaxValue;

            foreach (var mine in Mines)
            {
                if (MineToClaim(board.At(mine)))
                {
                    var tempDist = getDistance(pos, mine);
                    if (tempDist < meilleureDist)
                    {
                        meilleureDist = tempDist;
                        meilleurePos = mine;
                    }
                }
            }
            return meilleurePos;

        }

        public Pos GetClosestTavern(Pos pos)
        {
            Pos meilleurePos = new Pos();
            int meilleureDist = int.MaxValue;

            foreach (var taverne in Tavernes)
            {
                var tempDist = getDistance(pos, taverne);
                if (tempDist < meilleureDist)
                {
                    meilleureDist = tempDist;
                    meilleurePos = taverne;
                }
            }
            return meilleurePos;

        }


        private Tuple<string, int> CalculatePath(GameState state, Pos goal)
        {
            Pos start = state.myHero.pos;

            //Console.WriteLine("Begining calculating path from ({0},{1}) to ({2},{3})", start.x, start.y, goal.x, goal.y);

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
                        //Console.WriteLine("Path found! Going {0}", direction);
                        return new Tuple<string, int>(direction, currentVisited.weight);
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
            return new Tuple<string, int>(Direction.Stay, 0);
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

                if (!(currentTile == Tile.FREE || currentTile == Tile.SPIKES || areEqual(goal, current) || (currentTile >= Tile.HERO_1 && currentTile <= Tile.HERO_4)))
                {
                    return null;
                }

                if (previous != null)
                {
                    pc.weight = previous.weight; // to modify
                }

                pc.weight += getCost(state.board.At(current));
                pc.current = current;
                pc.previous = previous;
                pc.previousDirection = previousDirection;

                pc.heuristic = pc.weight + getDistance(current, goal);

                // Void path if it would kill us
                if (state.myHero.life < pc.weight)
                {
                    return null;
                }

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
                    cost = 10;
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