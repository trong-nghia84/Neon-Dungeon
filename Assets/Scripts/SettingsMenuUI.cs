using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện bắt buộc để chuyển cảnh

public class SettingsMenuUI : MonoBehaviour
{
    [Header("Cấu hình Tên Scene")]
    public string menuSceneName = "MenuScene"; // Đặt tên Scene Menu của bạn vào đây

    // 1. Hàm mở Menu (Dừng game)
    public void OpenSettingsMenu()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Đã tạm dừng game.");
    }

    // 2. Hàm đóng Menu (Tiếp tục game)
    public void CloseSettingsMenu()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        Debug.Log("Tiếp tục game.");
    }

    // 3. Hàm CHƠI LẠI (Restart Level hiện tại)
    public void RestartGame()
    {
        // QUAN TRỌNG: Phải trả thời gian về 1 trước khi load lại
        Time.timeScale = 1f;

        // Lấy tên của Scene hiện tại đang chơi và load lại chính nó
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);

        Debug.Log("Đang tải lại màn chơi hiện tại...");
    }

    // 4. Hàm TRỞ VỀ MENU CHÍNH
    public void GoToMainMenu()
    {
        // QUAN TRỌNG: Phải trả thời gian về 1 trước khi thoát ra Menu
        Time.timeScale = 1f;

        SceneManager.LoadScene(menuSceneName);
        Debug.Log("Đang quay lại Menu chính.");
    }

    private void OnDestroy()
    {
        // Bảo hiểm: Luôn trả timeScale về 1 khi Object này bị hủy
        Time.timeScale = 1f;
    }
}