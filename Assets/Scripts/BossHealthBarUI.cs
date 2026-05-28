using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBarUI : MonoBehaviour
{
    public Slider hpSlider;          // Kéo Boss_HP_Slider vào đây
    public TextMeshProUGUI nameText; // Kéo Boss_Name_Text vào đây (nếu có)

    private BossController activeBoss;


    void Start()
    {
        // Đảm bảo vừa vào game là thanh máu tự ẩn đi, chờ Boss gọi thì mới hiện
        gameObject.SetActive(false);
    }
    // Hàm để Boss gọi khi người chơi kích hoạt phòng Boss
    public void SetupBossHealthBar(BossController boss, string bossName)
    {
        activeBoss = boss;
        if (nameText != null) nameText.text = bossName;

        gameObject.SetActive(true); // Bật thanh máu lên
        UpdateHealthBar();
    }

    void Update()
    {
        // Liên tục cập nhật máu của Boss mỗi khung hình
        if (activeBoss != null)
        {
            UpdateHealthBar();

            // Nếu Boss chết thì ẩn thanh máu đi sau khi kết thúc
            if (activeBoss.isDead)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void UpdateHealthBar()
    {
        if (activeBoss != null && hpSlider != null)
        {
            // Lấy tỷ lệ máu hiện tại (Máu hiện tại / Máu tối đa)
            float hpRatio = activeBoss.GetHealthNormalized();
            hpSlider.value = hpRatio;
        }
    }
}