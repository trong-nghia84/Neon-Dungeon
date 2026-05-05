using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float health = 50f;
    public float damageToPlayer = 10f;
    public float moveSpeed = 3f;
    public float detectRange = 7f;
    public float stopDistance = 1.2f;

    protected Rigidbody2D rb;
    protected Animator anim;
    protected Transform player; // Sẽ được cập nhật liên tục
    protected PlayerSwitchManager switchManager;
    protected bool isDead = false;
    protected bool isFacingRight = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Tự động tìm PlayerSwitchManager trong Scene
        switchManager = GameObject.FindObjectOfType<PlayerSwitchManager>();

        // Thiết lập vật lý Top-down
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    protected virtual void Update()
    {
        // QUAN TRỌNG: Cập nhật mục tiêu theo nhân vật đang hoạt động từ Manager
        // Cách này giúp quái luôn bám theo đúng nhân vật đang hiện trên màn hình
        player = PlayerSwitchManager.CurrentPlayerTransform;

        // 1. Kiểm tra nếu quái chết hoặc không tìm thấy mục tiêu
        if (isDead || player == null)
        {
            StopMovement();
            return;
        }

        // 2. Kiểm tra trạng thái người chơi đã chết hết chưa
        if (switchManager != null && switchManager.isAllDead)
        {
            StopMovement();
            return;
        }
    }

    // Hàm bổ trợ để dừng quái vật một cách sạch sẽ
    protected void StopMovement()
    {
        if (rb != null) rb.velocity = Vector2.zero;
        if (anim != null) anim.SetFloat("Speed", 0);
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;

        if (anim != null) anim.SetTrigger("IsHurt");

        if (health <= 0) Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        StopMovement();

        if (anim != null) anim.SetTrigger("IsDie");

        // Vô hiệu hóa va chạm
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        this.enabled = false;

        Destroy(gameObject, 2f);
    }

    protected void Flip(float directionX)
    {
        if ((directionX > 0 && !isFacingRight) || (directionX < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }
}