using UnityEngine;
using TMPro;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Nội dung hướng dẫn")]
    [TextArea(3, 5)] // Tạo ô nhập chữ rộng rãi trong Inspector
    public string tutorialMessage;

    private TextMeshProUGUI uiText;
    private bool hasTriggered = false; // Cờ nếu bạn chỉ muốn hiện 1 lần duy nhất (Tùy chọn)

    void Awake()
    {
        // Tự động tìm linh hoạt đối tượng TextMeshProUGUI trên Canvas thông qua tên gọi
        GameObject textObj = GameObject.Find("TutorialText");
        if (textObj != null)
        {
            uiText = textObj.GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi Player bước vào vùng này
        if (collision.CompareTag("Player"))
        {
            if (uiText != null)
            {
                uiText.text = tutorialMessage; // Đổ chữ hướng dẫn lên màn hình
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Khi Player đi ra khỏi vùng này thì xóa chữ đi cho sạch màn hình
        if (collision.CompareTag("Player"))
        {
            if (uiText != null && uiText.text == tutorialMessage)
            {
                uiText.text = ""; // Xóa chữ
            }

            // Nếu Nghĩa muốn đi qua rồi thì vùng này tự hủy luôn, không cho hiện lại nữa:
            // Destroy(gameObject);
        }
    }
}