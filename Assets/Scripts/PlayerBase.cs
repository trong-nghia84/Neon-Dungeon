using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBase : MonoBehaviour
{
    [Header("Base Stats")]
    public float moveSpeed = 5f;

    protected Rigidbody2D rb;
    protected Animator anim;
    protected PlayerSwitchManager switchManager;
    protected bool isFacingRight = true;

    public bool isDead { get; protected set; } = false;

    private PlayerDash dashScript;
    public Collider2D hitboxCollider;

    [Header("Skill 1: Sword Wave (Phím R)")]
    public string skill1Name ;
    public float skill1Cooldown ; // Sóng kiếm hồi lâu (10s)
    public Sprite skill1Icon;
    protected float currentSkill1Timer = 0f;

    [Header("Skill 2: Block (Phím E)")]
    public string skill2Name ;
    public float skill2Cooldown ;  // Chặn hồi nhanh (3s)
    public Sprite skill2Icon;
    protected float currentSkill2Timer = 0f;

    public bool IsSkill1Ready => currentSkill1Timer <= 0f;
    public bool IsSkill2Ready => currentSkill2Timer <= 0f;

    [Header("Defense State")]
    public bool isInvincible = false; // Trạng thái bất tử tạm thời

    [Header("Shield Power-Up Settings")]
    public GameObject shieldVisualEffect; // Kéo đối tượng vòng sáng Neon (con của Player) vào đây

    [Header("Damage Buff Settings")]
    protected float currentDamageMultiplier = 1f; // Hệ số nhân mặc định là 1 (không tăng)
    private Coroutine damageBuffCoroutine;
    protected virtual void Awake()
    {
        dashScript = GetComponent<PlayerDash>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        switchManager = GameObject.FindObjectOfType<PlayerSwitchManager>();

        // Tự động tìm đối tượng con tên là "Hitbox" và lấy Collider của nó
        Transform hitboxTransform = transform.Find("CollectDamageBox");
        if (hitboxTransform != null)
        {
            hitboxCollider = hitboxTransform.GetComponent<Collider2D>();
        }
        
    }

    protected virtual void Update()
    {
        if (isDead) return;

        // LƯU Ý: Nếu đang lướt, ta chặn hoàn toàn logic di chuyển ở đây
        if (dashScript != null && dashScript.IsDashing)
        {
            anim.SetFloat("Speed", 0); // Đảm bảo không chạy anim chạy khi đang lướt
            return;
        }

        if (currentSkill1Timer > 0)
        {
            currentSkill1Timer -= Time.deltaTime;
        }

        if (currentSkill2Timer > 0)
        {
            currentSkill2Timer -= Time.deltaTime;
        }
    }

    public virtual void Move(Vector2 direction)
    {
        // LƯU Ý: Chặn thêm điều kiện IsDashing ở đây để an toàn tuyệt đối
        if (isDead || (dashScript != null && dashScript.IsDashing)) return;

        rb.velocity = direction * moveSpeed;

        if (anim != null)
            anim.SetFloat("Speed", direction.sqrMagnitude);

        if (direction.x > 0 && !isFacingRight) Flip();
        else if (direction.x < 0 && isFacingRight) Flip();
    }

    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    public abstract void Attack();

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        // ĐOẠN SỬA CHÍNH: Quái vẫn tấn công trúng, nhưng nếu có Khiên (isInvincible) thì chặn damage tại đây
        if (isInvincible)
        {
            Debug.Log(gameObject.name + " đang bật Khiên! Đạn/Quái đánh trúng nhưng không mất máu.");
            return; // Thoát hàm ngay lập tức, quái vẫn chơi anim đánh nhưng manager không bị trừ máu
        }

        if (switchManager != null)
        {
            switchManager.TakeSharedDamage(damage);

            if (anim != null) anim.SetTrigger("IsHurt");

            if (switchManager.isAllDead)
            {
                Die();
            }
        }
    }

    public virtual void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;

        if (anim != null) anim.SetTrigger("IsDie");

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        // Tắt luôn Hitbox con nếu có để chắc chắn không bị trúng đạn khi đã chết
        Transform hitbox = transform.Find("Hitbox");
        if (hitbox != null) hitbox.gameObject.SetActive(false);

        rb.isKinematic = true;
        gameObject.tag = "Untagged";
    }

    public virtual void UseSkill1()
    {
        if (IsSkill1Ready)
        {
            ExecuteSkill1Logic(); // Chạy chiêu thức
            currentSkill1Timer = skill1Cooldown; // Bắt đầu hồi chiêu
        }
    }

    public virtual void UseSkill2()
    {
        if (IsSkill2Ready)
        {
            ExecuteSkill2Logic(); // Chạy chiêu thức
            currentSkill2Timer = skill2Cooldown; // Bắt đầu hồi chiêu
        }
    }

    protected abstract void ExecuteSkill1Logic();
    protected abstract void ExecuteSkill2Logic();

    public float GetCooldownNormalized(int skillNumber)
    {
        switch (skillNumber)
        {
            case 1:
                return Mathf.Clamp01(currentSkill1Timer / skill1Cooldown);
            case 2:
                return Mathf.Clamp01(currentSkill2Timer / skill2Cooldown);
            default:
                return 0f;
        }
    }

    public void ActivateShieldPowerUp(float duration)
    {
        StartCoroutine(ShieldPowerUpRoutine(duration));
    }

    private System.Collections.IEnumerator ShieldPowerUpRoutine(float duration)
    {
        // 1. Bật trạng thái bất tử (Hàm TakeDamage hiện tại của bạn đã tự động chặn trừ máu khi biến này = true)
        isInvincible = true;

        // 2. Hiện hiệu ứng vòng neon bảo vệ
        if (shieldVisualEffect != null)
        {
            shieldVisualEffect.SetActive(true);
        }

        // 3. Chờ hết thời gian bổ trợ của Item
        yield return new WaitForSeconds(duration);

        // 4. Tắt trạng thái bất tử và ẩn hiệu ứng
        isInvincible = false;

        if (shieldVisualEffect != null)
        {
            shieldVisualEffect.SetActive(false);
        }

        Debug.Log("Hết thời gian hiệu lực của Khiên Bảo Vệ.");
    }

    public void ActivateDamageBuff(float multiplier, float duration)
    {
        // Nếu đang có buff cũ, dừng nó lại để tính thời gian mới
        if (damageBuffCoroutine != null) StopCoroutine(damageBuffCoroutine);

        damageBuffCoroutine = StartCoroutine(DamageBuffRoutine(multiplier, duration));
    }

    private System.Collections.IEnumerator DamageBuffRoutine(float multiplier, float duration)
    {
        currentDamageMultiplier = multiplier;

        // Phản hồi Game Feel: Đổi màu nhân vật sang màu cam/đỏ Neon để báo hiệu đang cuồng nộ
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.color = new Color(1f, 0.4f, 0f); // Màu cam rực

        yield return new WaitForSeconds(duration);

        // Hết thời gian buff, trả các chỉ số về mặc định
        currentDamageMultiplier = 1f;
        if (sprite != null) sprite.color = Color.white; // Trở lại màu gốc

        Debug.Log("Hết thời gian hiệu lực của Buff Tăng Sát Thương.");
    }

    public float GetDamageMultiplier()
    {
        return currentDamageMultiplier;
    }
}