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
    public float skill1Cooldown ; 
    public Sprite skill1Icon;
    protected float currentSkill1Timer = 0f;

    [Header("Skill 2: Block (Phím E)")]
    public string skill2Name ;
    public float skill2Cooldown ;  
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
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        switchManager = GameObject.FindObjectOfType<PlayerSwitchManager>();

        Transform hitboxTransform = transform.Find("CollectDamageBox");
        if (hitboxTransform != null)
        {
            hitboxCollider = hitboxTransform.GetComponent<Collider2D>();
        }
        
    }

    protected virtual void Update()
    {
        if (isDead) return;

        if (dashScript != null && dashScript.IsDashing)
        {
            anim.SetFloat("Speed", 0); 
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

        if (isInvincible)
        {
            Debug.Log(gameObject.name + " đang bật Khiên! Đạn/Quái đánh trúng nhưng không mất máu.");
            return; 
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

        Transform hitbox = transform.Find("Hitbox");
        if (hitbox != null) hitbox.gameObject.SetActive(false);

        rb.isKinematic = true;
        gameObject.tag = "Untagged";
    }

    public virtual void UseSkill1()
    {
        if (IsSkill1Ready)
        {
            ExecuteSkill1Logic(); 
            currentSkill1Timer = skill1Cooldown; 
        }
    }

    public virtual void UseSkill2()
    {
        if (IsSkill2Ready)
        {
            ExecuteSkill2Logic(); 
            currentSkill2Timer = skill2Cooldown; 
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

        Debug.Log("Hết thời gian hiệu lực của Khiên Bảo Vệ.");
    }

    public void ActivateDamageBuff(float multiplier, float duration)
    {
        if (damageBuffCoroutine != null) StopCoroutine(damageBuffCoroutine);

        damageBuffCoroutine = StartCoroutine(DamageBuffRoutine(multiplier, duration));
    }

    private System.Collections.IEnumerator DamageBuffRoutine(float multiplier, float duration)
    {
        currentDamageMultiplier = multiplier;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.color = new Color(1f, 0.4f, 0f); 

        yield return new WaitForSeconds(duration);

        currentDamageMultiplier = 1f;
        if (sprite != null) sprite.color = Color.white; 

        Debug.Log("Hết thời gian hiệu lực của Buff Tăng Sát Thương.");
    }

    public float GetDamageMultiplier()
    {
        return currentDamageMultiplier;
    }
}