using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerPlayer : PlayerBase
{
    public GameObject bulletPrefab; // Kéo Prefab viên đạn vào đây
    public Transform firePoint;     // Kéo Object FirePoint vào đây

    [Header("Gunner Skill Settings")]
    public GameObject bombPrefab;

    public Vector2 throwForce = new Vector2(5f, 3f);

    [Header("Stealth Settings")]
    public float stealthDuration = 4f; // Thời gian tàng hình (4 giây)
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Attack()
    {
        if (isDead) return;

        anim.SetTrigger("IsAttack"); // Chạy animation bắn
        //Shoot();
    }

    public void Shoot()
    {
        // Tạo viên đạn tại vị trí FirePoint với hướng xoay của nhân vật
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Nếu nhân vật đang quay trái (scale.x âm), xoay viên đạn ngược lại
        if (!isFacingRight)
        {
            bullet.transform.rotation = Quaternion.Euler(0, 0, 180f);
        }
    }

    protected override void ExecuteSkill1Logic()
    {

        // Kích hoạt Animation (Bạn dùng chung IsSkill1 hoặc Trigger riêng tùy ý)
        if (anim != null) anim.SetTrigger("IsSkill1");

        if (bombPrefab != null && firePoint != null)
        {
            // Tạo quả bom ngoài Scene
            GameObject bomb = Instantiate(bombPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();

            if (bombRb != null)
            {
                // Tính toán hướng lực ném dựa trên hướng mặt của Player
                Vector2 finalForce = throwForce;
                if (!isFacingRight)
                {
                    finalForce.x *= -1; // Ném ngược lại nếu quay mặt sang trái
                }

                // Tác dụng lực để quả bom bay theo đường vòng cung
                bombRb.AddForce(finalForce, ForceMode2D.Impulse);
            }
        }
    }

    protected override void ExecuteSkill2Logic()
    {

        //anim.SetTrigger("IsSkill2");
        StartCoroutine(StealthRoutine());
    }

    private IEnumerator StealthRoutine()
    {
        isInvincible = true;

        // 1. Làm mờ nhân vật (Chỉnh Alpha về 0.3)
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 0.3f;
            spriteRenderer.color = c;
        }

        // 2. Duy trì trong khoảng thời gian quy định
        yield return new WaitForSeconds(stealthDuration);

        // 3. Hết tàng hình, khôi phục lại trạng thái cũ
        isInvincible = false;

        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }
    }
}
