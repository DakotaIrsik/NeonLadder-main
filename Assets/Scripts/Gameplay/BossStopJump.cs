using Platformer.Core;
using Platformer.Mechanics;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the Jump Input is deactivated by the user, cancelling the upward velocity of the jump.
    /// </summary>
    /// <typeparam name="PlayerStopJump"></typeparam>
    public class BossStopJump : Simulation.Event<BossStopJump>
    {
        public BossController boss;

        public override void Execute()
        {

        }
    }
}