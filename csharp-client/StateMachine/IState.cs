using System.Runtime.CompilerServices;
using CoveoBlitz;
using CoveoBlitz.RandomBot;

namespace Coveo.StateMachine
{
    public abstract class IState
    {
        public virtual IState CalculateNextState(GameState state, TestBot bot)
        {
            return this;
        }

        public abstract Pos GetGoal(GameState state, TestBot bot);
    }
}