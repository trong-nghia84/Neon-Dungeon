using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    [Header("Melee Settings")]
    public GameObject attackHitbox;
    public float attackCooldown = 1.5f; 
    private float nextAttackTime = 0f;  

    protected override void Update()
    {
        base.Update();


        if (isDead || player == null || (switchManager != null && switchManager.isAllDead))
        {
            StopMoving(); 
            return;       
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectRange)
        {
            if (distanceToPlayer > stopDistance)
            {
                MoveTowardsPlayer();
            }
            else
            {
                if (Time.time >= nextAttackTime)
                {
                    AttackPlayer();
                }
                else
                {
                    StopMoving();
                }
            }
        }
        else
        {
            StopMoving();
        }
    }

    private void AttackPlayer()
    {
        rb.velocity = Vector2.zero;
        anim.SetFloat("Speed", 0f);

        anim.SetTrigger("IsAttack");

        nextAttackTime = Time.time + attackCooldown;
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        anim.SetFloat("Speed", rb.velocity.sqrMagnitude);
        Flip(direction.x);
    }

    private void StopMoving()
    {
        rb.velocity = Vector2.zero;
        anim.SetFloat("Speed", 0f);
    }

    public void EnableAttackHitbox() { if (attackHitbox != null) attackHitbox.SetActive(true); }
    public void DisableAttackHitbox() { if (attackHitbox != null) attackHitbox.SetActive(false); }
}

