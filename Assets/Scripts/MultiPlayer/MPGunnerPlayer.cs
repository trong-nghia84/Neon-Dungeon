using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MPGunnerPlayer : MPPlayerBase
{
    [Header("Attack")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Skill 1 - Bomb")]
    public GameObject bombPrefab;
    public Vector2 throwForce = new Vector2(5f, 3f);

    [Header("Skill 2 - Stealth")]
    public float stealthDuration = 4f;

    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Attack()
    {
        if (isDead)
            return;

        if (anim != null)
        {
            anim.SetTrigger("IsAttack");
        }
    }

    // Gọi từ Animation Event
    public void Shoot()
    {
        if (!IsServer)
            return;

        GameObject bullet =
            Instantiate(
                bulletPrefab,
                firePoint.position,
                firePoint.rotation);

        if (!isFacingRight)
        {
            bullet.transform.rotation =
                Quaternion.Euler(0, 0, 180f);
        }

        MPBulletManager bulletScript =
            bullet.GetComponent<MPBulletManager>();

        if (bulletScript != null)
        {
            bulletScript.OwnerClientId = OwnerClientId;
        }

        NetworkObject netObj =
            bullet.GetComponent<NetworkObject>();

        if (netObj != null)
        {
            netObj.Spawn();
        }
    }

    protected override void ExecuteSkill1Logic()
    {
        if (anim != null)
        {
            anim.SetTrigger("IsSkill1");
        }

        if (!IsServer)
            return;

        if (bombPrefab == null ||
            firePoint == null)
            return;

        GameObject bomb =
            Instantiate(
                bombPrefab,
                firePoint.position,
                Quaternion.identity);

        MPGunnerBomb bombScript =
            bomb.GetComponent<MPGunnerBomb>();

        if (bombScript != null)
        {
            bombScript.OwnerClientId =
                OwnerClientId;
        }

        Rigidbody2D bombRb =
            bomb.GetComponent<Rigidbody2D>();

        if (bombRb != null)
        {
            Vector2 finalForce =
                throwForce;

            if (!isFacingRight)
            {
                finalForce.x *= -1;
            }

            bombRb.AddForce(
                finalForce,
                ForceMode2D.Impulse);
        }

        NetworkObject netObj =
            bomb.GetComponent<NetworkObject>();

        if (netObj != null)
        {
            netObj.Spawn();
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
            Color c =
                spriteRenderer.color;

            c.a = 0.3f;

            spriteRenderer.color = c;
        }

        yield return new WaitForSeconds(
            stealthDuration);

        isInvincible = false;

        if (spriteRenderer != null)
        {
            Color c =
                spriteRenderer.color;

            c.a = 1f;

            spriteRenderer.color = c;
        }
    }
}