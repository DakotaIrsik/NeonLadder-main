using Assets.Scripts;
using Platformer.Mechanics.Stats;
using UnityEngine;

public class BossWeapon : MonoBehaviour
{
    public int attackDamage = 20;
    public int enragedAttackDamage = 40;
    public Vector3 attackOffset;
    public float attackRange = 1f;
    public LayerMask attackMask;

    private Transform attackOriginTransform;

    void Start()
    {
        // Use the parent's transform if available, otherwise use this object's transform
        attackOriginTransform = transform.parent != null ? transform.parent : transform;
    }

    public void Attack()
    {
        PerformAttack(attackDamage);
    }

    public void EnragedAttack()
    {
        PerformAttack(enragedAttackDamage);
    }

    private void PerformAttack(int damage)
    {
        Vector3 pos = attackOriginTransform.position + attackOriginTransform.right * attackOffset.x + attackOriginTransform.up * attackOffset.y;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, attackRange, attackMask);
        foreach (Collider2D colInfo in colliders)
        {
            if (colInfo != null && colInfo.tag == Constants.Player)
            {
                var healthComponent = colInfo.GetComponent<Health>();
                if (healthComponent != null)
                {
                    healthComponent.Decrement(damage);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackOriginTransform == null)
        {
            return;
        }

        Vector3 pos = attackOriginTransform.position + attackOriginTransform.right * attackOffset.x + attackOriginTransform.up * attackOffset.y;

        Gizmos.DrawWireSphere(pos, attackRange);
    }
}
