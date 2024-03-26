using Assets.Scripts;
using Platformer.Core;
using Platformer.Mechanics;

namespace Platformer.Gameplay
{
    public class BossDeath : Simulation.Event<BossDeath>
    {
        public BossController boss;

        public override void Execute()
        {

            Constants.DefeatedBosses.Add(boss.gameObject.name);
            boss._collider.enabled = false;
            boss.control.enabled = false;
            if (boss._audio && boss.ouch)
                boss._audio.PlayOneShot(boss.ouch);


        }
    }
}
