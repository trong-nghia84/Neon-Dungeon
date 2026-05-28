using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Cửa sổ chức năng (Panels)")]
    public GameObject multiPlayPanel; // Kéo MultiPlay_Panel vào đây
    public GameObject settingPanel;    // Kéo Setting_Panel vào đây

    [Header("Cấu hình Tên Scene Màn chơi")]
    public string levelSceneName = "LevelScene"; // Tên của Scene muốn chuyển đến

    void Start()
    {
        // Đảm bảo vừa vào Game (Start Scene) thì các Panel chức năng luôn ẩn đi
        if (multiPlayPanel != null) multiPlayPanel.SetActive(false);
        if (settingPanel != null) settingPanel.SetActive(false);

        // Đề phòng trường hợp từ màn chơi thoát về Menu, reset lại thời gian chạy bình thường
        Time.timeScale = 1f;
    }

    // =========================================================
    // LOGIC NÚT BẤM (BUTTON EVENTS)
    // =========================================================

    // 1. Nút PLAY: Chuyển sang màn chơi mới
    public void OnPlayButtonClick()
    {
        Debug.Log("Đang tải màn chơi: " + levelSceneName);
        SceneManager.LoadScene(levelSceneName);
    }

    // 2. Nút MULTIPLAY: Mở Panel MultiPlay
    public void OnMultiPlayButtonClick()
    {
        if (multiPlayPanel != null)
        {
            multiPlayPanel.SetActive(true);
            Debug.Log("Đã mở Panel MultiPlay.");
        }
    }

    // Hàm đóng Panel MultiPlay (Gắn vào nút Close bên trong Panel đó)
    public void CloseMultiPlayPanel()
    {
        if (multiPlayPanel != null)
        {
            multiPlayPanel.SetActive(false);
        }
    }

    // 3. Nút SETTING: Mở Panel Setting (Chặn tương tác bên ngoài)
    public void OnSettingButtonClick()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
            Debug.Log("Đã mở Panel Setting. Đã chặn tương tác các nút phía dưới.");
        }
    }

    // Hàm đóng Panel Setting (Gắn vào nút Close/X bên trong Panel Setting)
    public void CloseSettingPanel()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
    }

    // 4. Nút QUIT: Thoát Game hoàn toàn
    public void OnQuitButtonClick()
    {
        Debug.Log("Người chơi đã bấm thoát game!");

#if UNITY_EDITOR
        // Nếu đang chạy thử trong Unity Editor thì dừng PlayMode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Nếu là bản Build hoàn chỉnh thì tắt ứng dụng
        Application.Quit();
#endif
    }
}