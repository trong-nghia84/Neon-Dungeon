using UnityEngine;

public class ShootEnemy : EnemyBase
{
    [Header("Shooting Config")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 2f; 
    private float nextFireTime;

    protected override void Update()
    {
        base.Update();

        if (isDead || player == null) return;

        if (switchManager != null && switchManager.isAllDead)
        {
            StopMovement();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectRange)
        {
            HandleBehavior(distance);
        }
        else
        {
            StopMovement();
        }
    }

    private void HandleBehavior(float distance)
    {
        if (distance < stopDistance)
        {
            Vector2 retreatDir = (transform.position - player.position).normalized;
            rb.velocity = retreatDir * (moveSpeed * 0.8f);
            anim.SetFloat("Speed", 1);
        }
        else
        {
            StopMovement();
        }

        Flip(player.position.x - transform.position.x);

        if (Time.time >= nextFireTime)
        {
            if (anim != null) anim.SetTrigger("IsAttack");
            nextFireTime = Time.time + fireRate;
        }
    }

    public void Shoot()
    {

        if (bulletPrefab != null && firePoint != null && player != null)
        {
            Vector2 direction = (player.position - firePoint.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            BulletManager bManager = bullet.GetComponent<BulletManager>();
            if (bManager != null)
            {
                bManager.Launch(direction);
            }
            AudioManager.instance.PlaySFX("Gun");
        }
    }
}