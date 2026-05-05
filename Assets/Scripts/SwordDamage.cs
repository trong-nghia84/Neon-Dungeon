using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public float damageValue = 20f;

    [Header("Target Settings")]
    [Tooltip("Nhập Tag của đối tượng muốn gây sát thương (Player hoặc Enemy)")]
    public string targetTag = "Enemy";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra theo biến targetTag thay vì viết cứng chữ "Enemy"
        if (collision.CompareTag(targetTag))
        {
            // Nếu mục tiêu là Enemy
            if (targetTag == "Enemy")
            {
                EnemyBase enemy = collision.GetComponent<EnemyBase>();
                if (enemy != null) enemy.TakeDamage(damageValue);
            }
            // Nếu mục tiêu là Player
            else if (targetTag == "Player")
            {
                PlayerBase player = collision.GetComponent<PlayerBase>();
                if (player != null) player.TakeDamage(damageValue);
            }
        }
    }
}
