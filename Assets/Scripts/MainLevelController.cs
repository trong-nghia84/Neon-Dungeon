using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class MainLevelController : MonoBehaviour
{
    [Header("Cấu hình Tên Scene Quay lại Menu")]
    public string menuSceneName = "MenuScene";

    [Header("Cấu hình Tiến trình Cấp độ")]
    [Tooltip("Kéo thả lần lượt Level1But vào ô 0, Level2But vào ô 1... từ Hierarchy vào đây")]
    public Button[] levelButtons;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("ReachedLevel"))
        {
            PlayerPrefs.SetInt("ReachedLevel", 1);
            PlayerPrefs.Save();
        }

        UpdateLevelSelectionUI();
    }

    private void UpdateLevelSelectionUI()
    {
        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);
        Debug.Log("Tiến trình game hiện tại: Đã mở khóa đến Level " + reachedLevel);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] == null) continue;

            int levelNumber = i + 1; 

            if (levelNumber <= reachedLevel)
            {
                levelButtons[i].interactable = true; 

                Text btnText = levelButtons[i].GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    btnText.color = new Color(btnText.color.r, btnText.color.g, btnText.color.b, 1f);
                }
            }
            else
            {
                levelButtons[i].interactable = false; 

                Text btnText = levelButtons[i].GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    btnText.color = new Color(btnText.color.r, btnText.color.g, btnText.color.b, 0.25f);
                }
            }
        }
    }

    public void OnGoMenuButtonClick()
    {
        Debug.Log("Đang quay lại Menu chính: " + menuSceneName);
        SceneManager.LoadScene(menuSceneName);
    }

    public void OnSelectLevelButtonClick(string levelSceneName)
    {
        if (!string.IsNullOrEmpty(levelSceneName))
        {
            Debug.Log("Đang tải cấp độ: " + levelSceneName);
            SceneManager.LoadScene(levelSceneName);
        }
        else
        {
            Debug.LogWarning("Chưa nhập tên Scene cho nút bấm này!");
        }
    }

    [ContextMenu("Reset Progress")]
    public void ResetProgress()
    {
        PlayerPrefs.SetInt("ReachedLevel", 1);
        PlayerPrefs.Save();
        UpdateLevelSelectionUI();
        Debug.Log("Đã xóa dữ liệu tiến trình! Hoàn nguyên về Level 1.");
    }
}