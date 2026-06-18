using UnityEngine;
using UnityEngine.UI;

public class AudioToggleButton : MonoBehaviour
{
    private Image buttonImage;

    [Header("Gắn 2 hình ảnh của nút vào đây")]
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
        UpdateIcon();
    }

    // Gắn hàm này chung vào sự kiện On Click () của Button
    public void OnButtonPress()
    {
        // Gọi hàm của AudioManager
        AudioManager.instance.ToggleAudio();

        // Cập nhật lại hình ảnh của nút
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (AudioManager.instance.isMuted)
        {
            buttonImage.sprite = soundOffSprite;
        }
        else
        {
            buttonImage.sprite = soundOnSprite;
        }
    }
}