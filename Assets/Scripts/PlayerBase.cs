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
        else
        {
            Debug.LogWarning("Không tìm thấy đối tượng con tên là Hitbox trên " + gameObject.name);
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
        if (isDead || (switchManager != null && switchManager.isAllDead)) return;

        // LƯU Ý: Nếu đang lướt (Dash), thường chúng ta sẽ không nhận damage 
        // nhờ việc tắt Hitbox ở script PlayerDash. Nhưng để chắc chắn, 
        // có thể check thêm dashScript.IsDashing ở đây nếu muốn.

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
}