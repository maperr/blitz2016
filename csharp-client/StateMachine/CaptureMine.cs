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
            if (state.myHero.mineCount > 10000)
            {
                Console.WriteLine("Switching to attacking hero");
                return new AttackWinner();
            }
            return this;
        }

        public override Pos GetGoal(GameState state, TestBot bot)
        {
            Console.WriteLine("Capturing mine");
            return bot.GetClosestMine(state.myHero.pos, state.board);
        }
    }
}