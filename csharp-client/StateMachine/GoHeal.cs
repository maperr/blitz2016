using CoveoBlitz;
using CoveoBlitz.RandomBot;

namespace Coveo.StateMachine
{
    public class GoHeal : IState
    {
        public override IState CalculateNextState(GameState state, TestBot bot)
        {
            // Done Healing or no Cash
            if (state.myHero.life > 85 || state.myHero.gold == 0)
            {
                // Check if there is a good enemy to steal
                var maxMines = 0;
                foreach (var hero in state.heroes)
                {
                    if (maxMines < hero.mineCount)
                        maxMines = hero.mineCount;
                }

                // Go steal if he has more mine than you
                if (state.myHero.mineCount + 3 <= maxMines)
                {
                    return new AttackWinner();
                }

                // No worthy opponent, go capture mines
                return new CaptureMine();
            }

            // Go Heal
            return this;
        }

        public override Pos GetGoal(GameState state, TestBot bot)
        {
            // Go to tavern
            return bot.GetClosestTavern(state.myHero.pos);
        }
    }
}