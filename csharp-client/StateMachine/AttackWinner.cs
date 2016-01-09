using CoveoBlitz;
using CoveoBlitz.RandomBot;

namespace Coveo.StateMachine
{
    public class AttackWinner : IState
    {
        public override Pos GetGoal(GameState state, TestBot bot)
        {
            return state.heroes[1].pos;
        }
    }
}