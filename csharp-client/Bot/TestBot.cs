using System;
using System.Collections.Generic;
using System.Linq;

namespace CoveoBlitz.RandomBot
{
    public class TestBot : ISimpleBot
    {
        private readonly Random random = new Random();

        public int CostToTavern { get; set; }
        public int CostToMine { get; set; }
        public int Life { get; set; }
        public int Gold { get; set; }

        private bool setup = false;

        public void Setup()
        {
               
        }

        public void Shutdown()
        {
        }

        public string Move(GameState state)
        {
            // Initial setup
            if (!setup)
            {
                // Calculate cost to nearest tavern
                // Calculate cost to nearest mine
                setup = true;
            }

            // Update Info
            Life = state.myHero.life;
            Gold = state.myHero.gold;
            var pos = state.myHero.pos;
            var board = state.board;


            // If adjacent mine present
            if (pos.x > 1 && MineToClaim(board[pos.x - 1][pos.y], state.myHero.id))
                return Direction.North;
            if (pos.x < board.Length && MineToClaim(board[pos.x + 1][pos.y], state.myHero.id))
                return Direction.South;
            if (pos.y > 1 && MineToClaim(board[pos.x][pos.y - 1], state.myHero.id))
                return Direction.West;
            if (pos.y < board[0].Length && MineToClaim(board[pos.x][pos.y + 1], state.myHero.id))
                return Direction.East;

            // Check if life enough to subsist
            if (Life < CostToTavern + Constant.LifeDrainOnHit)
            {
                // Pathfind to tavern

            }
            else
            {
                // Pathfind to Mine
            }


            switch (random.Next(0, 5))
            {
                case 0:
                    return Direction.East;
                    break;

                case 1:
                    return Direction.West;
                    break;

                case 2:
                    return Direction.North;
                    break;

                case 3:
                    return Direction.South;
                    break;

                default:
                    return Direction.Stay;
            }

            return Direction.Stay;
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