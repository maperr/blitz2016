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
            // Target Enemy ?

            // if (costToMine + 25 >= life)
            // Heal

            // else
            // mine


            /*if (state.myHero.mineCount > 10000)
            {
                Console.WriteLine("Switching to attacking hero");
                return new AttackWinner();
            }*/

            if(state.myHero.life <= 30)
                return new GoHeal();


            // Max mine of hero
            var maxMines = 0;
            foreach (var hero in state.heroes)
            {
                if (maxMines < hero.mineCount)
                    maxMines = hero.mineCount;
            }

            if (state.myHero.mineCount + 3 <= maxMines)
            {
                if(state.myHero.life >= 75)
                    return new AttackWinner();
                else
                    return new GoHeal();
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