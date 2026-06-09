using UnityEngine;

public class DamageItem : MonoBehaviour
{
    [Header("Cấu hình Buff")]
    [Tooltip("Hệ số nhân sát thương, ví dụ: 2 là gấp đôi damage")]
    public float damageMultiplier = 2f;
    public float buffDuration = 7f; 

    [Header("Hiệu ứng")]
    public GameObject pickupVFX;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerBase player = collision.GetComponent<PlayerBase>();
            if (player == null) player = collision.GetComponentInParent<PlayerBase>();

            if (player != null)
            {
                player.ActivateDamageBuff(damageMultiplier, buffDuration);
                Debug.Log($"Player đã nhặt được Item Tăng Sát Thương x{damageMultiplier}!");
            }

            if (pickupVFX != null)
            {
                Instantiate(pickupVFX, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}