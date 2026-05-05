using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunnerPlayer : PlayerBase
{
    public GameObject bulletPrefab; // Kéo Prefab viên đạn vào đây
    public Transform firePoint;     // Kéo Object FirePoint vào đây

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
}
