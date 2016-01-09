using System;
using System.Linq;
using CoveoBlitz;
using CoveoBlitz.RandomBot;

namespace Coveo.StateMachine
{
    public class CaptureMine : IState
    {
        public override IState CalculateNextState(GameState state, TestBot bot)
        {
            if (state.myHero.mineCount > 0)
            {
                Console.WriteLine("Switching to attacking hero");
                return new AttackWinner();
            }
            return this;
        }

        public override Pos GetGoal(GameState state, TestBot bot)
        {
            Console.WriteLine("Capturing mine");

            int size = state.board.GetLength(0);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var pos = new Pos {x = i, y = j};
                    if (state.board.At(pos) == Tile.GOLD_MINE_NEUTRAL)
                    {
                        return pos;
                    }
                }
            }
            Console.WriteLine("Problem!!!");
            return null;
        }
    }
}