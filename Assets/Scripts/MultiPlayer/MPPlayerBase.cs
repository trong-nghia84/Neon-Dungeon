using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class MPPlayerBase : NetworkBehaviour
{
    [Header("Base Stats")]
    public float moveSpeed = 5f;

    protected Rigidbody2D rb;
    protected Animator anim;
    protected MPPlayerSwitchManager switchManager;
    protected bool isFacingRight = true;

    public bool isDead { get; protected set; } = false;

    private PlayerDash dashScript;
    public Collider2D hitboxCollider;

    [Header("Skill 1")]
    public string skill1Name;
    public float skill1Cooldown;
    public Sprite skill1Icon;

    protected float currentSkill1Timer = 0f;

    [Header("Skill 2")]
    public string skill2Name;
    public float skill2Cooldown;
    public Sprite skill2Icon;

    protected float currentSkill2Timer = 0f;

    public bool IsSkill1Ready => currentSkill1Timer <= 0f;
    public bool IsSkill2Ready => currentSkill2Timer <= 0f;

    [Header("Defense State")]
    public bool isInvincible = false;

    [Header("Shield Power-Up Settings")]
    public GameObject shieldVisualEffect;

    [Header("Damage Buff Settings")]
    protected float currentDamageMultiplier = 1f;

    private Coroutine damageBuffCoroutine;

    protected virtual void Awake()
    {
        dashScript = GetComponent<PlayerDash>();
        rb = GetComponentInParent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        switchManager =
            GetComponentInParent<MPPlayerSwitchManager>();

        Transform hitboxTransform =
            transform.Find("CollectDamageBox");

        if (hitboxTransform != null)
        {
            hitboxCollider =
                hitboxTransform.GetComponent<Collider2D>();
        }
    }

    protected virtual void Update()
    {
        if (isDead)
            return;

        if (dashScript != null &&
            dashScript.IsDashing)
        {
            if (anim != null)
            {
                anim.SetFloat("Speed", 0);
            }

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
        if (isDead) return;
        if (dashScript != null && dashScript.IsDashing) return;

        // THAY THẾ LỆNH RB.VELOCITY CŨ BẰNG LỆNH DI CHUYỂN KINEMATIC NÀY:
        if (rb != null)
        {
            // Tịnh tiến vị trí dựa trên vận tốc và thời gian vật lý FixedUpdate
            Vector2 moveVelocity = direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + moveVelocity);
        }

        // Đồng bộ hoạt ảnh di chuyển
        if (anim != null)
        {
            anim.SetFloat("Speed", direction.sqrMagnitude);
        }

        // Lật hình ảnh bằng flipX
        if (direction.x > 0 && !isFacingRight) Flip();
        else if (direction.x < 0 && isFacingRight) Flip();
    }

    protected void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 scale =
            transform.localScale;

        scale.x *= -1;

        transform.localScale = scale;
    }

    public abstract void Attack();

    public virtual void TakeDamage(float damage)
    {
        if (isDead)
            return;

        if (isInvincible)
            return;

        if (switchManager != null)
        {
            switchManager.TakeDamage(damage);

            if (anim != null)
            {
                anim.SetTrigger("IsHurt");
            }
        }
    }

    public virtual void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        if (anim != null)
        {
            anim.SetTrigger("IsDie");
        }

        Collider2D col =
            GetComponent<Collider2D>();

        if (col != null)
        {
            col.enabled = false;
        }

        Transform hitbox =
            transform.Find("Hitbox");

        if (hitbox != null)
        {
            hitbox.gameObject.SetActive(false);
        }

        gameObject.tag = "Untagged";
    }

    public virtual void UseSkill1()
    {
        if (!IsSkill1Ready)
            return;

        ExecuteSkill1Logic();

        currentSkill1Timer =
            skill1Cooldown;
    }

    public virtual void UseSkill2()
    {
        if (!IsSkill2Ready)
            return;

        ExecuteSkill2Logic();

        currentSkill2Timer =
            skill2Cooldown;
    }

    protected abstract void ExecuteSkill1Logic();
    protected abstract void ExecuteSkill2Logic();

    public float GetCooldownNormalized(
        int skillNumber)
    {
        switch (skillNumber)
        {
            case 1:
                return Mathf.Clamp01(
                    currentSkill1Timer /
                    skill1Cooldown);

            case 2:
                return Mathf.Clamp01(
                    currentSkill2Timer /
                    skill2Cooldown);

            default:
                return 0f;
        }
    }

    public void ActivateShieldPowerUp(
        float duration)
    {
        StartCoroutine(
            ShieldPowerUpRoutine(duration));
    }

    private IEnumerator ShieldPowerUpRoutine(
        float duration)
    {
        isInvincible = true;

        if (shieldVisualEffect != null)
        {
            shieldVisualEffect.SetActive(true);
        }

        yield return new WaitForSeconds(duration);

        isInvincible = false;

        if (shieldVisualEffect != null)
        {
            shieldVisualEffect.SetActive(false);
        }
    }

    public void ActivateDamageBuff(
        float multiplier,
        float duration)
    {
        if (damageBuffCoroutine != null)
        {
            StopCoroutine(
                damageBuffCoroutine);
        }

        damageBuffCoroutine =
            StartCoroutine(
                DamageBuffRoutine(
                    multiplier,
                    duration));
    }

    private IEnumerator DamageBuffRoutine(
        float multiplier,
        float duration)
    {
        currentDamageMultiplier =
            multiplier;

        SpriteRenderer sprite =
            GetComponent<SpriteRenderer>();

        if (sprite != null)
        {
            sprite.color =
                new Color(
                    1f,
                    0.4f,
                    0f);
        }

        yield return new WaitForSeconds(duration);

        currentDamageMultiplier = 1f;

        if (sprite != null)
        {
            sprite.color = Color.white;
        }
    }

    public float GetDamageMultiplier()
    {
        return currentDamageMultiplier;
    }

    public bool IsOwnerPlayer()
    {
        return switchManager != null &&
               switchManager.IsOwner;
    }
}