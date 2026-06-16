using UnityEngine;

public class ShieldItem : MonoBehaviour
{
    [Header("Cài đặt Item")]
    public float shieldDuration = 5f; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerBase player = collision.GetComponent<PlayerBase>();
            if (player == null) player = collision.GetComponentInParent<PlayerBase>();

            if (player != null)
            {
                player.ActivateShieldPowerUp(shieldDuration);

                Debug.Log("Player đã nhặt được Khiên Phòng Thủ!");
            }

            Destroy(gameObject);
        }
    }
}