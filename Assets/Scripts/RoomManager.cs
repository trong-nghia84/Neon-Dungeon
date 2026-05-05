using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GateController[] gates; // Kéo các cổng vào đây
    private bool roomCleared = false;

    void Update()
    {
        if (roomCleared) return;

        // Kiểm tra xem còn đối tượng con (Enemy) nào không
        // Cách đơn giản nhất là đếm số lượng Transform con
        if (transform.childCount == 0)
        {
            roomCleared = true;
            OpenRoomGate();
        }
    }

    void OpenRoomGate()
    {
        if (gates != null)
        {
            foreach (var gate in gates)
            {
                gate.OpenGate();
            }
            // Bạn có thể thêm âm thanh báo hiệu Clear Room ở đây
        }
    }
}