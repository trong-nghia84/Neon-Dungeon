using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerPlayer : PlayerBase
{
    public GameObject bulletPrefab; 
    public Transform firePoint;     

    [Header("Gunner Skill Settings")]
    public GameObject bombPrefab;

    public Vector2 throwForce = new Vector2(5f, 3f);

    [Header("Stealth Settings")]
    public float stealthDuration = 4f; 
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Attack()
    {
        if (isDead) return;

        anim.SetTrigger("IsAttack"); 
        //Shoot();
    }

    public void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (!isFacingRight)
        {
            bullet.transform.rotation = Quaternion.Euler(0, 0, 180f);
        }
    }

    protected override void ExecuteSkill1Logic()
    {

        if (anim != null) anim.SetTrigger("IsSkill1");

        if (bombPrefab != null && firePoint != null)
        {
            GameObject bomb = Instantiate(bombPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();

            if (bombRb != null)
            {
                Vector2 finalForce = throwForce;
                if (!isFacingRight)
                {
                    finalForce.x *= -1; 
                }

                bombRb.AddForce(finalForce, ForceMode2D.Impulse);
            }
        }
    }

    protected override void ExecuteSkill2Logic()
    {

        StartCoroutine(StealthRoutine());
    }

    private IEnumerator StealthRoutine()
    {
        isInvincible = true;

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 0.3f;
            spriteRenderer.color = c;
        }

        yield return new WaitForSeconds(stealthDuration);

        isInvincible = false;

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }
    }
}
