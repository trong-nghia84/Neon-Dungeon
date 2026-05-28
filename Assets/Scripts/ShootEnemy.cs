using UnityEngine;

public class ShootEnemy : EnemyBase
{
    [Header("Shooting Config")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 2f; // Tốc độ bắn (giây/viên)
    private float nextFireTime;

    protected override void Update()
    {
        // Gọi Update của EnemyBase để cập nhật mục tiêu player
        base.Update();

        if (isDead || player == null) return;

        if (switchManager != null && switchManager.isAllDead)
        {
            StopMovement();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // Chỉ hoạt động khi player nằm trong tầm phát hiện
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
        // 1. Logic Di chuyển
        if (distance < stopDistance)
        {
            // Nếu player quá gần, lùi lại để né
            Vector2 retreatDir = (transform.position - player.position).normalized;
            rb.velocity = retreatDir * (moveSpeed * 0.8f);
            anim.SetFloat("Speed", 1);
        }
        else
        {
            // Đủ khoảng cách an toàn thì đứng lại bắn
            StopMovement();
        }

        // Luôn xoay mặt về phía player
        Flip(player.position.x - transform.position.x);

        // 2. Logic Tấn công
        if (Time.time >= nextFireTime)
        {
            if (anim != null) anim.SetTrigger("IsAttack");
            nextFireTime = Time.time + fireRate;
        }
    }

    public void Shoot()
    {
        // Không đặt anim.SetTrigger ở đây nữa vì đã đặt ở Update rồi

        if (bulletPrefab != null && firePoint != null && player != null)
        {
            // 1. Tính toán hướng từ nòng súng tới vị trí hiện tại của Player
            Vector2 direction = (player.position - firePoint.position).normalized;

            // 2. Sinh đạn ra tại nòng súng
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            // 3. Lấy script BulletManager và kích hoạt chế độ bay vật lý (Launch)
            BulletManager bManager = bullet.GetComponent<BulletManager>();
            if (bManager != null)
            {
                bManager.Launch(direction);
            }

        }
    }
}