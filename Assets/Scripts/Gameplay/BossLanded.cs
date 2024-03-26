using Platformer.Core;
using Platformer.Mechanics;
using Platformer.UI;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player character lands after being airborne.
    /// </summary>
    /// <typeparam name="PlayerLanded"></typeparam>
    public class BossLanded : Simulation.Event<BossLanded>
    {
        public BossController boss;

        public override void Execute()
        {
        }
    }
}