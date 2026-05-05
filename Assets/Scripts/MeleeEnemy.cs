using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyBase
{
    [Header("Melee Settings")]
    public GameObject attackHitbox;
    public float attackCooldown = 1.5f; // Thời gian nghỉ giữa 2 lần chém (giây)
    private float nextAttackTime = 0f;  // Mốc thời gian được phép đánh tiếp

    protected override void Update()
    {
        base.Update();


        // NẾU NGƯỜI CHƠI ĐÃ CHẾT (kiểm tra qua Manager mà ta đã thiết lập ở lớp cha)
        // Hoặc nếu quái đã chết/mất mục tiêu
        if (isDead || player == null || (switchManager != null && switchManager.isAllDead))
        {
            StopMoving(); // Đảm bảo quái đứng yên
            return;       // THOÁT HÀM NGAY LẬP TỨC, không chạy code bên dưới
        }

        // --- PHẦN CODE BÊN DƯỚI CHỈ CHẠY KHI PLAYER CÒN SỐNG ---
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectRange)
        {
            if (distanceToPlayer > stopDistance)
            {
                MoveTowardsPlayer();
            }
            else
            {
                // Chỉ tấn công khi đã hết thời gian hồi chiêu
                if (Time.time >= nextAttackTime)
                {
                    AttackPlayer();
                }
                else
                {
                    // Trong lúc chờ hồi chiêu thì đứng yên
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

        // Kích hoạt Animation
        anim.SetTrigger("IsAttack");

        // Cập nhật mốc thời gian cho lần đánh sau
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

    // Các hàm Animation Event giữ nguyên như cũ
    public void EnableAttackHitbox() { if (attackHitbox != null) attackHitbox.SetActive(true); }
    public void DisableAttackHitbox() { if (attackHitbox != null) attackHitbox.SetActive(false); }
}

