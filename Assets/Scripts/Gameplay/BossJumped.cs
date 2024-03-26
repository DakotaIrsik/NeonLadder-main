using Platformer.Core;
using Platformer.Mechanics;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player performs a Jump.
    /// </summary>
    /// <typeparam name="PlayerJumped"></typeparam>
    public class BossJumped : Simulation.Event<BossJumped>
    {
        public BossController boss;

        public override void Execute()
        {
            if (boss.audioSource && boss.bossActions.jumpAudio)
                boss.audioSource.PlayOneShot(boss.bossActions.jumpAudio);
        }
    }
}