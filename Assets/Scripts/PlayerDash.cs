using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerBase playerBase; // Tham chiếu để lấy hitboxCollider
    private bool canDash = true;
    public bool IsDashing { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerBase = GetComponent<PlayerBase>();
    }

    void Update()
    {
        if (IsDashing) return;

        // Đổi phím từ LeftShift sang E
        if (Input.GetKeyDown(KeyCode.E) && canDash)
        {
            Debug.Log("Da nhan phim E");
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        IsDashing = true;

        // Bất tử: Tắt Hitbox thông qua PlayerBase
        if (playerBase != null && playerBase.hitboxCollider != null)
            playerBase.hitboxCollider.enabled = false;

        if (anim != null) anim.SetTrigger("IsDash");

        // TÍNH TOÁN HƯỚNG LƯỚT THEO HƯỚNG DI CHUYỂN
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 dashDir = new Vector2(moveX, moveY).normalized;

        // Nếu người chơi đứng yên (không nhấn phím di chuyển), lướt theo hướng mặt nhân vật
        if (dashDir == Vector2.zero)
        {
            dashDir = new Vector2(transform.localScale.x > 0 ? 1 : -1, 0);
        }

        rb.velocity = dashDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        // Kết thúc lướt
        IsDashing = false;
        if (playerBase != null && playerBase.hitboxCollider != null)
            playerBase.hitboxCollider.enabled = true;

        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}