using CoveoBlitz;
using CoveoBlitz.RandomBot;

namespace Coveo.StateMachine
{
    public class GoHeal : IState
    {
        public override IState CalculateNextState(GameState state, TestBot bot)
        {
            if (state.myHero.life > 75)
            {
                var maxMines = 0;
                foreach (var hero in state.heroes)
                {
                    if (maxMines < hero.mineCount)
                        maxMines = hero.mineCount;
                }

                if (state.myHero.mineCount + 3 <= maxMines)
                {
                    return new AttackWinner();
                }

                return new CaptureMine();
            }

            return this;
        }

        public override Pos GetGoal(GameState state, TestBot bot)
        {
            return bot.GetClosestTavern(state.myHero.pos);
        }
    }
}