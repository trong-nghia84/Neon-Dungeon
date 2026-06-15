using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MPSwordDamage : MonoBehaviour
{
    public float damageAmount = 20f; // Lượng sát thương của một cú chém
    private MPPlayerSwitchManager mySwitchManager;

    private void Awake()
    {
        // Tìm bộ quản lý tổng của nhân vật sở hữu thanh kiếm này
        mySwitchManager = GetComponentInParent<MPPlayerSwitchManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // QUAN TRỌNG: Chỉ có Server mới được quyền xử lý va chạm gây sát thương
        if (!NetworkManager.Singleton.IsServer)
            return;

        // Tìm xem vật thể va chạm có phải là một bộ phận của Player không
        MPPlayerBase victim = collision.GetComponent<MPPlayerBase>();
        if (victim == null)
        {
            victim = collision.GetComponentInParent<MPPlayerBase>();
        }

        if (victim != null)
        {
            // Tìm bộ quản lý tổng của nạn nhân
            MPPlayerSwitchManager victimManager = victim.GetComponentInParent<MPPlayerSwitchManager>();

            if (victimManager != null)
            {
                // BẢO HIỂM: Không tự chém trúng chính mình (So sánh OwnerClientId mạng)
                if (victimManager.OwnerClientId == mySwitchManager.OwnerClientId)
                    return;

                // Thực hiện trừ máu nạn nhân trực tiếp trên Server!
                Debug.Log($"[CHIẾN ĐẤU] Người chơi ID {mySwitchManager.OwnerClientId} chém trúng người chơi ID {victimManager.OwnerClientId}!");
                victim.TakeDamage(damageAmount);
            }
        }
    }
}
