using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    [Header("Cấu hình Ô Skill")]
    [Tooltip("Điền số 1 cho ô Skill 1, số 2 cho ô Skill 2")]
    public int skillNumber = 1;

    [Header("UI References")]
    public Image skillIconImage; // Kéo đối tượng Skill1_Icon (hoặc Skill2_Icon) vào đây để đổi hình ảnh
    public Image cooldownImage;  // Kéo đối tượng Skill1_Cooldown (lớp phủ tối) vào đây để xử lý vòng hồi chiêu

    void Start()
    {
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f;
        }
    }

    void Update()
    {
        // 1. Kiểm tra xem có nhân vật nào đang đứng trên sân không
        if (PlayerSwitchManager.CurrentPlayerTransform != null)
        {
            PlayerBase activePlayer = PlayerSwitchManager.CurrentPlayerTransform.GetComponent<PlayerBase>();

            if (activePlayer != null)
            {
                // ==========================================
                // XỬ LÝ ĐỔI HÌNH ẢNH ICON THEO NHÂN VẬT
                // ==========================================
                if (skillIconImage != null)
                {
                    // Lấy đúng Sprite Icon đã cấu hình từ nhân vật đang hoạt động
                    Sprite targetIcon = (skillNumber == 1) ? activePlayer.skill1Icon : activePlayer.skill2Icon;

                    // Nếu nhân vật có gắn Icon, cập nhật nó lên màn hình UI
                    if (targetIcon != null)
                    {
                        skillIconImage.sprite = targetIcon;
                    }
                }

                // ==========================================
                // XỬ LÝ THỜI GIAN COOLDOWN THEO NHÂN VẬT
                // ==========================================
                if (cooldownImage != null)
                {
                    // Lấy tỷ lệ % hồi chiêu chuẩn hóa (từ 0 đến 1) riêng biệt của nhân vật đó
                    float cooldownPercentage = activePlayer.GetCooldownNormalized(skillNumber);

                    // Cập nhật lớp phủ tối quay vòng
                    cooldownImage.fillAmount = cooldownPercentage;
                }
            }
        }
    }
}