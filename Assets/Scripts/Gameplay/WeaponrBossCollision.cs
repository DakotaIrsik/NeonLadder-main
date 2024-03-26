using Platformer.Mechanics;
using Platformer.Mechanics.Stats;
using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Gameplay
{
    public class WeaponBossCollision : Event<WeaponBossCollision>
    {
        public BossController boss { get; set; }
        public PlayerController player { get; set; }
        public Collision2D collider { get; set; }

        PlatformerModel model = GetModel<PlatformerModel>();

        private bool willHurt { get; set; }

        public override void Execute()
        {
            willHurt = IsPlayerAttacking();

            //AppLogger.Logger.Information($"Will Hurt Enemy: {willHurt}");
            var enemyCollider = boss.GetComponent<BoxCollider2D>();
            if (!enemyCollider.isTrigger)
            {
                if (willHurt)
                {
                    var enemyHealth = boss.GetComponent<Health>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.Decrement();
                        if (!enemyHealth.IsAlive)
                        {
                            Schedule<BossDeath>().boss = boss;
                            //player.Bounce(2);
                        }
                        else
                        {
                            //player.Bounce(7);
                        }
                    }
                    else
                    {
                        Schedule<BossDeath>().boss = boss;
                        //player.Bounce(2);
                    }
                }
            }
        }

        private bool IsPlayerAttacking()
        {
            var isAttacking = player.animator.GetBool("isAttacking");
            //AppLogger.Logger.Information($"Is Player Attacking: {isAttacking}"); // Log the state
            return isAttacking;
        }
    }
}
