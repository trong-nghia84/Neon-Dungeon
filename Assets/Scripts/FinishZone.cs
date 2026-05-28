using UnityEngine;

public class FinishZone : MonoBehaviour
{
    [Header("Cấu hình Chỉ số Cấp độ hiện tại")]
    [Tooltip("Nếu đây là màn 1 thì điền số 1, màn 2 điền số 2... ngay trong Inspector nhé")]
    public int currentLevelNumber = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"Người chơi đã chạm đích! Vượt qua Level {currentLevelNumber} thành công.");

            int nextLevel = currentLevelNumber + 1;

            int currentReached = PlayerPrefs.GetInt("ReachedLevel", 1);

            if (nextLevel > currentReached)
            {
                PlayerPrefs.SetInt("ReachedLevel", nextLevel);
                PlayerPrefs.Save(); // Cưỡng chế ghi dữ liệu xuống ổ cứng vĩnh viễn
                Debug.Log($"[TIẾN TRÌNH] Đã lưu kỷ lục mới thành công! Mở khóa: Level {nextLevel}");
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerVictory();
            }
        }
    }
}