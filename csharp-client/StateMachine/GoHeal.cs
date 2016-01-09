using CoveoBlitz;
using CoveoBlitz.RandomBot;

namespace Coveo.StateMachine
{
    public class GoHeal : IState
    {
        public override Pos GetGoal(GameState state, TestBot bot)
        {
            return bot.GetClosestTavern(state.myHero.pos);
        }
    }
}