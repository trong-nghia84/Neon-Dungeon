using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI; // Thêm thư viện để điều khiển nút bấm nếu cần
using UnityEngine.SceneManagement;

public class UINetworkManager : MonoBehaviour
{
    public TMP_InputField joinCodeInput;
    public TMP_Text roomCodeText;
    public GameObject networkPanel;

    [Header("Cấu hình Tên Scene Đấu Trường 1v1")]
    [Tooltip("Điền chính xác tên Scene thực chiến 1v1 của bạn vào đây")]
    public string playSceneName = "SoloScene";

    private void Start()
    {
        // Đăng ký sự kiện lắng nghe kết nối mạng
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    // HÀM TỰ ĐỘNG CHẠY KHI ĐỦ NGƯỜI VÀ XỬ LÝ CHUYỂN CẢNH MẠNG
    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[MẠNG] Người chơi có ID: {clientId} đã kết nối vào phòng!");

        // CHỈ CÓ HOST (SERVER) MỚI CÓ QUYỀN KIỂM TRA ĐIỀU KIỆN ĐỂ CHUYỂN CẢNH VÀO TRẬN
        if (NetworkManager.Singleton.IsServer)
        {
            // Nếu tổng số người trong phòng đã đủ 2 (Gồm Host và 1 Client vừa vào)
            if (NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                Debug.Log("Phòng đã đủ 2 người! Hệ thống bắt đầu kích hoạt chuyển cảnh mạng...");

                if (networkPanel != null) networkPanel.SetActive(false);

                // Gọi lệnh chuyển cảnh mạng tối cao của Netcode
                NetworkManager.Singleton.SceneManager.LoadScene(playSceneName, LoadSceneMode.Single);
            }
        }
        else
        {
            // Đối với máy Client: Khi thấy mình kết nối thành công vào phòng
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                if (networkPanel != null) networkPanel.SetActive(false);
                roomCodeText.text = "Đã vào phòng! Đang chờ Chủ phòng tải trận đấu...";
            }
        }
    }

    // Nút bấm Tạo phòng (Dành cho BẤT KỲ MÁY NÀO muốn làm Host)
    public async void CreateRoom()
    {
        roomCodeText.text = "Đang khởi tạo phòng trên Relay...";

        string code = await RelayManager.Instance.CreateRelay();

        roomCodeText.text = "MÃ PHÒNG CỦA BẠN: " + code;
        Debug.Log("Đã tạo phòng thành công với mã: " + code);
    }

    // Nút bấm Vào phòng (Dành cho BẤT KỲ MÁY NÀO muốn làm Client nhập mã của bạn bè)
    public async void JoinRoom()
    {
        string inputCode = joinCodeInput.text.ToUpper().Trim();

        if (string.IsNullOrEmpty(inputCode))
        {
            roomCodeText.text = "Vui lòng nhập mã phòng!";
            return;
        }

        roomCodeText.text = "Đang kết nối tới phòng: " + inputCode;

        await RelayManager.Instance.JoinRelay(inputCode);
    }

    public void Home()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene("MenuScene");
    }
}