using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton để các script khác (Player, Gate) dễ dàng gọi tới
    public static GameManager Instance { get; private set; }

    [Header("Giao diện UI Panels")]
    public GameObject gameOverPanel;  // Kéo Panel Thua vào đây
    public GameObject victoryPanel;   // Kéo Panel Thắng vào đây

    [Header("Cấu hình Tên Scene")]
    public string menuSceneName = "MenuScene";
    public string nextLevelSceneName = "Level2"; // Tên của Level tiếp theo

    void Awake()
    {
        // Khởi tạo Singleton
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        // Ẩn cả 2 Panel khi mới vào màn chơi
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        Time.timeScale = 1f; // Đảm bảo thời gian chạy bình thường
    }

 
    public void TriggerGameOver()
    {
        Debug.Log("Player đã hết máu! Game Over.");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); // Hiện bảng thua
            Time.timeScale = 0f;          // Đóng băng game
        }
    }

  
    public void TriggerVictory()
    {
        Debug.Log("Chúc mừng! Bạn đã phá đảo map này.");
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);  // Hiện bảng thắng
            Time.timeScale = 0f;          // Đóng băng game
        }
    }

   

    // Nút Tiếp tục (Chỉ có ở Panel Thắng)
    public void NextLevel()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }
        else
        {
            Debug.LogWarning("Chưa cấu hình tên Cấp độ tiếp theo trong GameManager!");
        }
    }

    // Nút Chơi lại (Có ở cả 2 Panel)
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    // Nút Về Menu (Có ở cả 2 Panel)
    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}