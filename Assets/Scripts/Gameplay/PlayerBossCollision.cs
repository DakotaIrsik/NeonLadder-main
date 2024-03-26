using Assets.Scripts;
using Platformer.Mechanics;
using Platformer.Mechanics.Stats;
using Platformer.Model;
using UnityEngine;
using static Platformer.Core.Simulation;
using Serilog;

namespace Platformer.Gameplay
{
    public class PlayerBossCollision : Event<PlayerBossCollision>
    {
        public BossController boss;
        public PlayerController player;
        public Collision2D collider;

        PlatformerModel model = GetModel<PlatformerModel>();

        private bool willHurtEnemy { get; set; }

        private bool willHurtPlayer { get; set; }

        public override void Execute()
        {
            willHurtEnemy = (player.Bounds.center.y >= boss.Bounds.max.y) ||
                            (collider.collider.tag == Constants.PlayerWeapon && IsPlayerAttacking());

            willHurtPlayer = IsBossAttacking();

            //AppLogger.Logger.Information($"Will Hurt Enemy: {willHurtEnemy}, Will Hurt Player: {willHurtPlayer}");
            //AppLogger.Logger.Information($"Boss Perspective Collider Name: {collider?.collider?.name ?? "NULL NAME"} \r\n Boss Perspective Collider Tag {collider?.collider?.tag ?? "NULL TAG"}");
            

            //get enemy's capsulecollider2d
            var enemyCollider = boss.GetComponent<BoxCollider2D>();
            if (!enemyCollider.isTrigger)
            {
                if (willHurtEnemy)
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

                if (willHurtPlayer)
                {
                    player.health.Decrement();
                    //player.playerActions.knockback = true;
                }
            }
        }

        private bool IsBossAttacking()
        {
            var isAttacking = boss.animator.GetBool("isAttacking");
            //AppLogger.Logger.Information($"Is Boss Attacking: {isAttacking}"); // Log the state
            return isAttacking;
        }

        private bool IsPlayerAttacking()
        {
            var isAttacking = player.animator.GetBool("isAttacking");
            //AppLogger.Logger.Information($"Is Player Attacking: {isAttacking}"); // Log the state
            return isAttacking;
        }
    }
}
