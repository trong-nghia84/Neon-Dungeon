using UnityEngine;

public class TilemapDamageTrap : MonoBehaviour
{
    [Header("Cấu hình Sát thương bẫy")]
    public float trapDamage = 10f;       // Lượng máu bị đốt mỗi lần
    public float burnInterval = 1f;      // Chu kỳ đốt máu (Cứ 1 giây đốt 1 lần)

    private float nextDamageTime = 0f;   // Biến lưu mốc thời gian cho phát đốt tiếp theo

    // Hàm này được Unity gọi LIÊN TỤC mỗi khung hình khi có thực thể nằm TRONG vùng Collider Trigger
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Kiểm tra nếu đối tượng đang dẫm lên bẫy là Player
        if (collision.CompareTag("FootPlayer"))
        {
            // Kiểm tra xem đã đến mốc thời gian được phép đốt máu tiếp theo chưa (đảm bảo giãn cách đúng 1 giây)
            if (Time.time >= nextDamageTime)
            {
                ExecuteTrapDamage();

                // Cập nhật lại mốc thời gian cho phát đốt kế tiếp = Thời gian hiện tại + 1 giây
                nextDamageTime = Time.time + burnInterval;
            }
        }
    }

    // Hàm thực thi trừ máu tổng và kiểm tra sinh tử
    private void ExecuteTrapDamage()
    {
        // 1. Kiểm tra an toàn hệ thống quản lý máu tổng
        if (PlayerSwitchManager.Instance != null)
        {
            // 2. LẤY NHÂN VẬT ĐANG HOẠT ĐỘNG THỰC TẾ để kiểm tra trạng thái bất tử (isInvincible)
            // Cách này giúp lấy chính xác biến ẩn/hiển Hitbox từ PlayerBase
            PlayerBase activePlayer = null;
            if (PlayerSwitchManager.CurrentPlayerTransform != null)
            {
                activePlayer = PlayerSwitchManager.CurrentPlayerTransform.GetComponent<PlayerBase>();
            }

            // 3. SỬA LỖI: Kiểm tra trạng thái bất tử từ activePlayer 
            // và thay đổi toàn bộ 'currentHealth' thành 'sharedHealth' cho đúng với file Manager của bạn
            if (activePlayer != null && !activePlayer.isInvincible)
            {
                // Thay đổi thành TakeSharedDamage để trừ máu chuẩn thông qua hàm có sẵn của bạn
                PlayerSwitchManager.Instance.TakeSharedDamage(trapDamage);

                Debug.Log($"[BẪY TILEMAP] Đang đốt máu! Trừ {trapDamage} HP. Máu còn lại: {PlayerSwitchManager.Instance.sharedHealth}");
            }
            else if (activePlayer == null)
            {
                // Trường hợp dự phòng nếu chưa kịp gán CurrentPlayerTransform, vẫn trừ máu thẳng qua hàm dùng chung
                PlayerSwitchManager.Instance.TakeSharedDamage(trapDamage);
            }
            else
            {
                Debug.Log("[BẪY TILEMAP] Player đang lướt bất tử hoặc có Khiên, triệt tiêu sát thương đốt!");
            }
        }
    }

    // Hàm này reset lại mốc thời gian khi người chơi vừa bước ra khỏi bẫy rồi quay lại
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Cho phép bẫy đốt máu ngay lập tức ở giây đầu tiên khi vừa chạm chân vào bẫy
            nextDamageTime = Time.time;
        }
    }
}