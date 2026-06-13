using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItems : MonoBehaviour
{
    [Header("Cấu hình hồi máu")]
    public float healAmount = 25f; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerSwitchManager manager = GameObject.FindObjectOfType<PlayerSwitchManager>();

            if (manager != null && manager.sharedHealth < manager.maxHealth)
            {
                manager.HealSharedHealth(healAmount);

                Destroy(gameObject);
            }
        }
    }
}
