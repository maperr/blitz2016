using System;
using CoveoBlitz;
using CoveoBlitz.RandomBot;

namespace Coveo.StateMachine
{
    public class AttackWinner : IState
    {
        public override Pos GetGoal(GameState state, TestBot bot)
        {
            Console.WriteLine("This is war!!");
            return state.heroes[1].pos;
        }
    }
}