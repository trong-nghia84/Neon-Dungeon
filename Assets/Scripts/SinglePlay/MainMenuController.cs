using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Cửa sổ chức năng (Panels)")]
    public GameObject multiPlayPanel; 
    public GameObject settingPanel;   

    [Header("Cấu hình Tên Scene Màn chơi")]
    public string levelSceneName = "LevelScene"; 

    void Start()
    {
        if (multiPlayPanel != null) multiPlayPanel.SetActive(false);
        if (settingPanel != null) settingPanel.SetActive(false);

        Time.timeScale = 1f;
    }

   

    public void OnPlayButtonClick()
    {
        Debug.Log("Đang tải màn chơi: " + levelSceneName);
        SceneManager.LoadScene(levelSceneName);
    }

    public void OnMultiPlayButtonClick()
    {
        if (multiPlayPanel != null)
        {
            multiPlayPanel.SetActive(true);
            Debug.Log("Đã mở Panel MultiPlay. Đã chặn tương tác các nút phía dưới.");
        }
    }

    public void OnSettingButtonClick()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
            Debug.Log("Đã mở Panel Setting. Đã chặn tương tác các nút phía dưới.");
        }
    }

    public void CloseSettingPanel()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
    }

    public void OnQuitButtonClick()
    {
        Debug.Log("Người chơi đã bấm thoát game!");
        Application.Quit();
    }
}