using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using System.Collections;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player has died.
    /// </summary>
    public class PlayerDeath : Simulation.Event<PlayerDeath>
    {
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        float DeathAnimationDuration = 1.1f;

        public override void Execute()
        {
            var player = model.player;
            player.controlEnabled = false;
            player.animator.SetBool("dead", true);
            player.StartCoroutine(HandleDeathAnimation(player));
        }

        private IEnumerator HandleDeathAnimation(PlayerController player)
        {
            yield return new WaitForSeconds(DeathAnimationDuration);
            player.animator.enabled = false;

            // Optionally, you can schedule a respawn or other event here
            // Simulation.Schedule<PlayerSpawn>(2);
        }
    }
}
