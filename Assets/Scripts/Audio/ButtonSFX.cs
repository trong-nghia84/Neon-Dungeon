using UnityEngine;
using UnityEngine.UI;

public class ButtonSFX : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        // Tự động tìm component Button gắn trên chính GameObject này
        button = GetComponent<Button>();

        if (button != null)
        {
            // Tự động đăng ký sự kiện: Khi nút được bấm, sẽ gọi hàm PlayClickSound
            button.onClick.AddListener(PlayClickSound);
        }
    }

    private void PlayClickSound()
    {
        // Gọi AudioManager phát âm thanh có tên là "ButtonClick" (tên bạn đã đặt ở Bước 1)
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX("Button");
        }
    }
}