using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; }

    [Header("Giao diện UI Panels")]
    public GameObject gameOverPanel; 
    public GameObject victoryPanel;   

    [Header("Cấu hình Tên Scene")]
    public string menuSceneName = "MenuScene";
    public string nextLevelSceneName = "Level2"; 

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        Time.timeScale = 1f; 
    }

 
    public void TriggerGameOver()
    {
        Debug.Log("Player đã hết máu! Game Over.");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); 
            Time.timeScale = 0f;          
        }
    }

  
    public void TriggerVictory()
    {
        Debug.Log("Chúc mừng! Bạn đã phá đảo map này.");
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);  
            Time.timeScale = 0f;          
        }
    }

   

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

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}