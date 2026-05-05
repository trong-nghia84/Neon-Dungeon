using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItems : MonoBehaviour
{
    [Header("Cấu hình hồi máu")]
    public float healAmount = 25f; // Lượng máu sẽ cộng thêm

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra va chạm với Player (hoặc Hitbox của Player)
        if (collision.CompareTag("Player"))
        {
            // Tìm Manager quản lý máu chung
            PlayerSwitchManager manager = GameObject.FindObjectOfType<PlayerSwitchManager>();

            if (manager != null && manager.sharedHealth < manager.maxHealth)
            {
                // Thực hiện hồi máu
                manager.HealSharedHealth(healAmount);

                // Hủy vật phẩm ngay sau khi ăn
                Destroy(gameObject);
            }
        }
    }
}
