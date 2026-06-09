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
    public EnemyHealthBar healthBar; 
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
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    protected virtual void Update()
    {
        player = PlayerSwitchManager.CurrentPlayerTransform;

        if (player != null)
        {
            PlayerBase playerScript = player.GetComponent<PlayerBase>();

            if (playerScript == null) playerScript = player.GetComponentInParent<PlayerBase>();

            if (playerScript != null && playerScript.isInvincible)
            {
                player = null;
            }
        }

        if (isDead || player == null)
        {
            StopMovement();
            return;
        }

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

        }
    }
}