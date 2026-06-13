using UnityEngine;
using UnityEngine.SceneManagement; 

public class SettingsMenuUI : MonoBehaviour
{
    [Header("Cấu hình Tên Scene")]
    public string menuSceneName = "MenuScene"; 
    public void OpenSettingsMenu()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Đã tạm dừng game.");
    }

    public void CloseSettingsMenu()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
        Debug.Log("Tiếp tục game.");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);

        Debug.Log("Đang tải lại màn chơi hiện tại...");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(menuSceneName);
        Debug.Log("Đang quay lại Menu chính.");
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}