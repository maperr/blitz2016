using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<Pos> Mines { get; set; } 

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
                Mines = GetMinePos(state.board);
                var distances = Mines.Select(x => (x.x - pos.x) ^ 2 + (x.y - pos.y) ^ 2).ToList();
                var closestMine = Mines.ElementAt(distances.ToList().IndexOf(distances.Min()));
                Console.WriteLine("Mine la plus proche :" + closestMine.x + ", "+closestMine.y);

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
                count ++;
            } while (BadChoice(direction, pos, board) && count < 4);
            if(count < 4)
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

        public List<Pos> GetMinePos(Tile[][] board)
        {
            var positions = new List<Pos>();
            for (var i = 0; i < board.Length; i++)
            {
                for (var j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] >= Tile.GOLD_MINE_NEUTRAL && board[i][j] <= Tile.GOLD_MINE_4)
                    {
                        positions.Add(new Pos() {x = i, y =j});
                    }
                }
            }
            return positions;
        }
    }
}