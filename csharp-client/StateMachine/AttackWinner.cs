using System;
using CoveoBlitz;
using CoveoBlitz.RandomBot;

namespace Coveo.StateMachine
{
    public class AttackWinner : IState
    {
        public override IState CalculateNextState(GameState state, TestBot bot)
        {
            var enemy = GetEnemy(state);

            if (enemy.mineCount <= state.myHero.mineCount)
            {
                if(state.myHero.life > 25)
                    return new CaptureMine();
                return new GoHeal();
            }
            else if(enemy.life > state.myHero.life)
                return new GoHeal();

            return this;
        }

        public override Pos GetGoal(GameState state, TestBot bot)
        {
            var enemy = GetEnemy(state);
            return enemy.pos;
        }

        public Hero GetEnemy(GameState state)
        {
            var EnemyPlayer = new Hero();
            var MaxMine = 0;

            foreach (var hero in state.heroes)
            {
                if (hero.mineCount > MaxMine)
                {
                    if (hero.pos != state.myHero.pos)
                    {
                        MaxMine = hero.mineCount;
                        EnemyPlayer = hero;
                    }
                }
            }

            return EnemyPlayer;
        }
    }
}