using Platformer.Mechanics;
using UnityEngine;

public class BossEncounter : StateMachineBehaviour
{
    private GameObject player;
    private PlayerController playerController;
    private Rigidbody2D rb;
    private BossController bossController;
    private float delayBeforeChase = 0.5f; // Delay in seconds
    private float lastSwitchTime;
    private float attackRange = 1.5f; // The range within which the boss should switch to attack
    private bool playerIsInAttackRange = true;
    private bool targetDead = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.Find("Protagonist");
        playerController = player.GetComponent<PlayerController>();
        rb = animator.GetComponent<Rigidbody2D>();
        bossController = animator.GetComponent<BossController>();
        lastSwitchTime = Time.time; // Initialize the last switch time
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("targetDead", !playerController.health.IsAlive);
        targetDead = animator.GetBool("targetDead");

        if (!targetDead)
        {
            animator.SetBool("engaged", true);
        }

        if (!targetDead && (Time.time > lastSwitchTime + delayBeforeChase))
        {
            bossController.moveDirection = (int)Mathf.Sign(player.transform.position.x - rb.position.x);
            lastSwitchTime = Time.time; // Reset the last switch time
        }

        playerIsInAttackRange = IsWithinAttackRange();
        if (playerIsInAttackRange && !targetDead)
        {
            animator.SetBool("isAttacking", true);
            animator.GetComponentInChildren<BossWeapon>().Attack();
        }
    }

    private bool IsWithinAttackRange(bool log = false)
    {
        float distanceToPlayer = Vector2.Distance(rb.position, player.transform.position);
        if (log)
        {
            Debug.Log($"Distance to player: {distanceToPlayer}");
        }
        return distanceToPlayer <= attackRange;

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        animator.SetBool("engaged", false);
        animator.SetBool("isAttacking", false);
        //return control to parent and set idle animation of 13?);
        animator.SetInteger("animation", 13);
        bossController.moveDirection = 0;
    }
}
