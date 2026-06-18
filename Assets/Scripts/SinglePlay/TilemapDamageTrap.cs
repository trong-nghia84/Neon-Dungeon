using UnityEngine;

public class TilemapDamageTrap : MonoBehaviour
{
    [Header("Cấu hình Sát thương bẫy")]
    public float trapDamage = 10f;       
    public float burnInterval = 1f;      

    private float nextDamageTime = 0f;   

    
    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.CompareTag("FootPlayer"))
        {
            if (Time.time >= nextDamageTime)
            {
                ExecuteTrapDamage();
                nextDamageTime = Time.time + burnInterval;
            }
        }
    }
   
    private void ExecuteTrapDamage()
    {
        if (PlayerSwitchManager.Instance != null)
        {
            PlayerBase activePlayer = null;
            if (PlayerSwitchManager.CurrentPlayerTransform != null)
            {
                activePlayer = PlayerSwitchManager.CurrentPlayerTransform.GetComponent<PlayerBase>();
            }

            if (activePlayer != null && !activePlayer.isInvincible)
            {
                PlayerSwitchManager.Instance.TakeSharedDamage(trapDamage);

                Debug.Log($"[BẪY TILEMAP] Đang đốt máu! Trừ {trapDamage} HP. Máu còn lại: {PlayerSwitchManager.Instance.sharedHealth}");
            }
            else if (activePlayer == null)
            {
                PlayerSwitchManager.Instance.TakeSharedDamage(trapDamage);
            }
            else
            {
                Debug.Log("[BẪY TILEMAP] Player đang lướt bất tử hoặc có Khiên, triệt tiêu sát thương đốt!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            nextDamageTime = Time.time;
        }
    }
}