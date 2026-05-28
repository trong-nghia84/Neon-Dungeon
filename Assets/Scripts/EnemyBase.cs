using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    public float maxHealth = 50f; // Đổi thành maxHealth để quản lý Slider dễ hơn
    protected float currentHealth;
    public float damageToPlayer = 10f;
    public float moveSpeed = 3f;
    public float detectRange = 7f;
    public float stopDistance = 1.2f;

    [Header("UI References")]
    public EnemyHealthBar healthBar; // Kéo script EnemyHealthBar vào đây trong Inspector

    protected Rigidbody2D rb;
    protected Animator anim;
    protected Transform player;
    protected PlayerSwitchManager switchManager;
    public bool isDead = false;
    protected bool isFacingRight = true;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        switchManager = GameObject.FindObjectOfType<PlayerSwitchManager>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
    }

    protected virtual void Start()
    {
        // 1. Khởi tạo máu hiện tại
        currentHealth = maxHealth;

        // 2. Thiết lập giá trị cho thanh máu UI
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    protected virtual void Update()
    {
        player = PlayerSwitchManager.CurrentPlayerTransform;

        // 2. KIỂM TRA TÀNG HÌNH: Nếu tìm thấy Player nhưng Player đang bật kỹ năng tàng hình
        if (player != null)
        {
            PlayerBase playerScript = player.GetComponent<PlayerBase>();

            // Nếu không tìm thấy script ở đối tượng chính, tìm thử ở đối tượng cha (đề phòng cấu trúc Canvas/Collider con)
            if (playerScript == null) playerScript = player.GetComponentInParent<PlayerBase>();

            if (playerScript != null && playerScript.isInvincible)
            {
                // Ép player về null để quái "mất dấu" người chơi hoàn toàn
                player = null;
            }
        }

        // 3. Nếu quái chết, hoặc không có Player, hoặc Player đang tàng hình thì dừng lại
        if (isDead || player == null)
        {
            StopMovement();
            return;
        }

        // 4. Nếu toàn đội Player đã chết (Game Over) thì quái cũng dừng lại
        if (switchManager != null && switchManager.isAllDead)
        {
            StopMovement();
            return;
        }
    }

    protected void StopMovement()
    {
        if (rb != null) rb.velocity = Vector2.zero;
        if (anim != null) anim.SetFloat("Speed", 0);
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // 3. Cập nhật thanh máu UI mỗi khi trúng đòn
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (anim != null) anim.SetTrigger("IsHurt");

        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        StopMovement();

        // 4. Ẩn thanh máu ngay khi chết để không bị treo lơ lửng
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        if (anim != null) anim.SetTrigger("IsDie");

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

            // Mẹo: Nếu bạn không dùng Quaternion.identity trong script HealthBar, 
            // bạn có thể lật ngược lại localScale của HealthBar ở đây để nó luôn xuôi chiều.
        }
    }
}