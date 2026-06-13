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
        if (collision.CompareTag(targetTag))
        {

            float finalDamage = damageValue;

            if (PlayerSwitchManager.CurrentPlayerTransform != null)
            {
                PlayerBase activePlayer = PlayerSwitchManager.CurrentPlayerTransform.GetComponent<PlayerBase>();
                if (activePlayer != null)
                {
                    finalDamage = damageValue * activePlayer.GetDamageMultiplier();
                }
            }

            if (targetTag == "Enemy")
            {
                EnemyBase enemy = collision.GetComponent<EnemyBase>();
                if (enemy == null) enemy = collision.GetComponentInParent<EnemyBase>();

                if (enemy != null)
                {
                    enemy.TakeDamage(finalDamage);
                    Debug.Log($"Kiếm chém trúng Enemy gây: {finalDamage} sát thương.");
                }
            }
            else if (targetTag == "Player")
            {
                PlayerBase player = collision.GetComponentInParent<PlayerBase>();
                if (player == null) player = collision.GetComponent<PlayerBase>();

                if (player != null)
                {
                    player.TakeDamage(damageValue);
                }
            }
        }
    }
}