using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // CẦN THÊM DÒNG NÀY ĐỂ ĐIỀU KHIỂN ĐƯỢC THÀNH PHẦN BUTTON

public class MainLevelController : MonoBehaviour
{
    [Header("Cấu hình Tên Scene Quay lại Menu")]
    public string menuSceneName = "MenuScene";

    [Header("Cấu hình Tiến trình Cấp độ")]
    [Tooltip("Kéo thả lần lượt Level1But vào ô 0, Level2But vào ô 1... từ Hierarchy vào đây")]
    public Button[] levelButtons;

    private void Start()
    {
        // 1. Kiểm tra nếu là lần đầu chạy game, thiết lập mặc định đã mở khóa đến Level 1
        if (!PlayerPrefs.HasKey("ReachedLevel"))
        {
            PlayerPrefs.SetInt("ReachedLevel", 1);
            PlayerPrefs.Save();
        }

        // 2. Chạy hàm cập nhật trạng thái làm mờ / bật tắt nút bấm
        UpdateLevelSelectionUI();
    }

    // Hàm tự động quét danh sách nút để bật/tắt hoặc làm mờ dựa trên dữ liệu PlayerPrefs
    private void UpdateLevelSelectionUI()
    {
        // Lấy chỉ số màn cao nhất người chơi đã đạt được (mặc định là 1)
        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);
        Debug.Log("Tiến trình game hiện tại: Đã mở khóa đến Level " + reachedLevel);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            // Nếu Nghĩa quên chưa kéo thả nút vào mảng trong Inspector thì bỏ qua để tránh lỗi Log đặc
            if (levelButtons[i] == null) continue;

            int levelNumber = i + 1; // Số màn thực tế (1, 2, 3, 4, 5)

            if (levelNumber <= reachedLevel)
            {
                // TRƯỜNG HỢP 1: Màn này đã được mở khóa hợp pháp
                levelButtons[i].interactable = true; // Cho phép bấm nút

                // Làm chữ số hiển thị trên nút sáng rõ ràng (Kênh Alpha = 1)
                Text btnText = levelButtons[i].GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    btnText.color = new Color(btnText.color.r, btnText.color.g, btnText.color.b, 1f);
                }
            }
            else
            {
                // TRƯỜNG HỢP 2: Màn này chưa chơi tới (Bị khóa)
                levelButtons[i].interactable = false; // Vô hiệu hóa tương tác (Nút tự động chuyển sang Disabled Color)

                // Ép font số điện tử của nút mờ hẳn đi (Hạ Alpha xuống 25%) để tạo hiệu ứng khóa trực quan
                Text btnText = levelButtons[i].GetComponentInChildren<Text>();
                if (btnText != null)
                {
                    btnText.color = new Color(btnText.color.r, btnText.color.g, btnText.color.b, 0.25f);
                }
            }
        }
    }

    // 1. Hàm quay lại Menu chính (Giữ nguyên logic cũ của bạn)
    public void OnGoMenuButtonClick()
    {
        Debug.Log("Đang quay lại Menu chính: " + menuSceneName);
        SceneManager.LoadScene(menuSceneName);
    }

    // 2. HÀM THÔNG MINH: Chuyển đến bất kỳ Level nào dựa vào tên truyền vào (Giữ nguyên logic cũ của bạn)
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

    // TÍNH NĂNG BỔ TRỢ: Chuột phải vào Script trong Inspector chọn "Reset Progress" nếu muốn quay lại Level 1 để test game
    [ContextMenu("Reset Progress")]
    public void ResetProgress()
    {
        PlayerPrefs.SetInt("ReachedLevel", 1);
        PlayerPrefs.Save();
        UpdateLevelSelectionUI();
        Debug.Log("Đã xóa dữ liệu tiến trình! Hoàn nguyên về Level 1.");
    }
}