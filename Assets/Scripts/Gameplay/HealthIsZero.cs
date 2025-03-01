using Platformer.Mechanics.Stats;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player health reaches 0. This usually would result in a 
    /// PlayerDeath event.
    /// </summary>
    /// <typeparam name="HealthIsZero"></typeparam>
    public class HealthIsZero : Event<HealthIsZero>
    {
        public Health health;

        public override void Execute()
        {

            switch (health.gameObject.tag)
            {
                case "Player":
                    Schedule<PlayerDeath>();
                    break;
                case "Enemy":
                    Schedule<EnemyDeath>();
                    break;
                default:
                    //Debug.Log(health.gameObject.tag);
                    break;

            }

        }
    }
}