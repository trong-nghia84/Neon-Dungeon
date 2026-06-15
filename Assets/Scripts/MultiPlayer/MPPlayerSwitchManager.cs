using Cinemachine;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MPPlayerSwitchManager : NetworkBehaviour
{
    [Header("Characters")]
    public MPPlayerBase[] characters;

    private MPPlayerBase currentActiveCharacter;

    private NetworkVariable<int> activeCharacterIndex =
        new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    [Header("Health")]
    public float maxHealth = 100f;

    private NetworkVariable<float> currentHealth =
        new NetworkVariable<float>(
            100f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

    [Header("UI")]
    public Slider healthSlider;
    public GameObject winUI;  
    public GameObject loseUI; 

    [Header("Camera")]
    public CinemachineVirtualCamera vcam;

    public static MPPlayerSwitchManager LocalPlayer;

    public Vector3 targetPos;

    public override void OnNetworkSpawn()
    {
        activeCharacterIndex.OnValueChanged += OnCharacterChanged;
        currentHealth.OnValueChanged += OnHealthChanged;

        if (IsServer)
        {
            currentHealth.Value = maxHealth;
            activeCharacterIndex.Value = 0;

            
        }
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEventManaged;
        }

        if (IsOwner)
        {
            LocalPlayer = this;

            // Nếu ngay từ đầu đã ở SoloScene (trường hợp test trực tiếp trong Editor)
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SoloScene")
            {
                InitializeLocalPlayerComponents();
                Invoke(nameof(TeleportToSpawnPoint), 0.2f);
            }
        }

        // Bật nhân vật con lên tại chỗ trước
        ActivateCharacter(activeCharacterIndex.Value);
        UpdateHealthUI();
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEventManaged;
        }
    }

    // Hàm xử lý sự kiện nạp cảnh của Netcode
    private void OnSceneEventManaged(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted)
        {
            // Kiểm tra chắc chắn rằng Scene vừa nạp xong chính là Đấu trường SoloScene của bạn
            if (sceneEvent.SceneName == "SoloScene")
            {
                Debug.Log($"[MẠNG] Đã chuyển sang SoloScene thành công! Tiến hành kích hoạt tìm kiếm thành phần.");

                // ÉP BUỘC CHẠY: Gọi hàm tự động quét tìm Slider và Camera cho người chơi Local
                if (IsOwner)
                {
                    InitializeLocalPlayerComponents();
                }

                // Đưa nhân vật về đúng vị trí Spawn Point sòng phẳng
                TeleportClientRpc();
            }
        }
    }

    [ClientRpc]
    private void TeleportClientRpc()
    {
        TeleportToSpawnPoint();
    }

    private int scanRetries = 0; // Biến đếm số lần quét thử lại
    private void InitializeLocalPlayerComponents()
    {
        Debug.Log($"[MẠNG CHI TIẾT] Đang tiến hành quét tìm SlideHealth, UI Thắng Thua và Virtual Camera... (Lần thử: {scanRetries + 1})");

        bool foundAll = true;

        // 1. Tìm Slider máu dựa trên tên chính xác "SliderHealth" hoặc "SlideHealth"
        // Nhìn vào ảnh mới nhất, tên chuẩn trên máy của Nghĩa là "SliderHealth" nha!
        GameObject foundSlider = GameObject.Find("SliderHealth");
        if (foundSlider != null)
        {
            healthSlider = foundSlider.GetComponent<Slider>();
            Debug.Log("[UI MẠNG] Đã liên kết thành công Slider máu!");
        }
        else
        {
            healthSlider = FindObjectOfType<Slider>(true);
        }
        if (healthSlider == null) foundAll = false;


        // ====================================================================
        // THUẬT TOÁN ĐỔI MỚI CHÍ MẠNG: ÉP QUÉT SÂU TÌM UI ĐANG ẨN KHÔNG LO BỊ NULL
        // Chúng ta quét trực tiếp trong toàn bộ tài nguyên Scene bao gồm cả các vật thể đã bị ẩn
        if (winUI == null || loseUI == null)
        {
            foreach (UnityEngine.Transform t in FindObjectsOfType<UnityEngine.Transform>(true))
            {
                if (t.name == "GameWinUI")
                {
                    winUI = t.gameObject;
                }
                if (t.name == "GameLoseUI")
                {
                    loseUI = t.gameObject;
                }
            }
        }

        // Sau khi đã gán chắc chắn vào biến, ta mới ra lệnh ẩn tụi nó đi để đầu trận sạch màn hình
        if (winUI != null) winUI.SetActive(false);
        if (loseUI != null) loseUI.SetActive(false);

        // Kiểm tra an toàn: Nếu ép quét diện rộng rồi mà vẫn rỗng, đánh dấu để tìm lại
        if (winUI == null || loseUI == null) foundAll = false;
        // ====================================================================


        // 2. Tìm Cinemachine Virtual Camera (Giữ nguyên code cũ)
        if (vcam == null)
        {
            vcam = FindObjectOfType<CinemachineVirtualCamera>(true);
        }

        if (vcam != null)
        {
            Debug.Log("[CAMERA MẠNG] Đã liên kết thành công Virtual Camera!");
            if (currentActiveCharacter != null)
            {
                vcam.Follow = currentActiveCharacter.transform;
            }
        }
        else
        {
            foundAll = false;
        }

        if (healthSlider != null) UpdateHealthUI();

        // Cơ chế bảo hiểm quét lại nâng cấp
        if (!foundAll && scanRetries < 10)
        {
            scanRetries++;
            Invoke(nameof(InitializeLocalPlayerComponents), 0.1f);
            Debug.LogWarning("[MẠNG CẢNH BÁO] Chưa gom đủ linh kiện. Đang chuẩn bị quét lại...");
        }
        else if (foundAll)
        {
            scanRetries = 0;
            Debug.Log("[MẠNG THÀNH CÔNG] Toàn bộ UI Thắng Thua, Slider và Camera đã được cấu hình hoàn chỉnh!");
        }
    }

    private void Update()
    {
        
        if (!IsOwner)
            return;

        if (currentHealth.Value <= 0)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            AttackServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchCharacterServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Skill1ServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Skill2ServerRpc();
        }
    }

    private void FixedUpdate()
    {
        // Bảo hiểm tối cao: Chỉ có máy ĐANG SỞ HỮU nhân vật này mới được phép điều khiển di chuyển
        if(!IsOwner)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.fixedDeltaTime * 10);
        }
        if (!IsOwner || currentActiveCharacter == null || currentHealth.Value <= 0)
        {
            return;
        }

        // Đọc dữ liệu bàn phím trực tiếp
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDir = new Vector2(moveX, moveY).normalized;

        // Gọi hàm di chuyển vật lý của nhân vật hiện tại
        currentActiveCharacter.Move(moveDir);
        SendPositionServerRpc(transform.position);
    }

    [ServerRpc]
    private void AttackServerRpc()
    {
        AttackClientRpc();
    }

    [ClientRpc]
    private void AttackClientRpc()
    {
        if (currentActiveCharacter != null)
            currentActiveCharacter.Attack();
    }

    [ServerRpc]
    private void Skill1ServerRpc()
    {
        Skill1ClientRpc();
    }

    [ClientRpc]
    private void Skill1ClientRpc()
    {
        if (currentActiveCharacter != null)
            currentActiveCharacter.UseSkill1();
    }

    [ServerRpc]
    private void Skill2ServerRpc()
    {
        Skill2ClientRpc();
    }

    [ClientRpc]
    private void Skill2ClientRpc()
    {
        if (currentActiveCharacter != null)
            currentActiveCharacter.UseSkill2();
    }

    [ServerRpc]
    private void SwitchCharacterServerRpc()
    {
        activeCharacterIndex.Value =
            (activeCharacterIndex.Value + 1)
            % characters.Length;
    }

    private void OnCharacterChanged(int oldIndex, int newIndex)
    {
        ActivateCharacter(newIndex);
    }

    private void ActivateCharacter(int index)
    {
        Vector3 pos = transform.position;

        for (int i = 0; i < characters.Length; i++)
        {
            bool active = (i == index);

            characters[i].gameObject.SetActive(active);

            if (active)
            {
                currentActiveCharacter = characters[i];
                currentActiveCharacter.transform.position = pos;

                if (IsOwner && vcam != null)
                {
                    vcam.Follow =
                        currentActiveCharacter.transform;
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (!IsServer)
            return;

        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0;
            DieClientRpc();
        }
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        // 1. Kích hoạt hoạt ảnh chết của nhân vật con (Giữ nguyên code cũ của bạn)
        if (currentActiveCharacter != null)
        {
            currentActiveCharacter.Die();
        }

        // THUẬT TOÁN ĐỒNG BỘ UI THẮNG THUA CHUẨN ĐÍCH DANH LOCAL PLAYER (Đã cập nhật nâng cấp)
        if (IsOwner)
        {
            // Kiểm tra bảo hiểm quét lại nếu bị hụt
            if (winUI == null || loseUI == null)
            {
                foreach (UnityEngine.Transform t in FindObjectsOfType<UnityEngine.Transform>(true))
                {
                    if (t.name == "GameWinUI") winUI = t.gameObject;
                    if (t.name == "GameLoseUI") loseUI = t.gameObject;
                }
            }

            // TỰ CHECK MÁU CỦA CHÍNH MÌNH ĐỂ HIỂN THỊ UI VÀ ÉP LAYER LÊN TRÊN CÙNG
            if (currentHealth.Value <= 0)
            {
                if (loseUI != null)
                {
                    loseUI.SetActive(true);

                    // LỆNH TỐI CAO: Ép bảng UI Thua cuộc nhảy xuống đáy danh sách Canvas để đè lên trên toàn bộ các UI khác!
                    loseUI.transform.SetAsLastSibling();
                }
                Debug.Log("[KẾT THÚC THỰC TẾ] Máu của tôi bằng 0. Tôi THUA!");
            }
            else
            {
                if (winUI != null)
                {
                    winUI.SetActive(true);

                    // LỆNH TỐI CAO: Ép bảng UI Chiến thắng nhảy xuống đáy danh sách Canvas để đè lên trên toàn bộ các UI khác!
                    winUI.transform.SetAsLastSibling();
                }
                Debug.Log("[KẾT THÚC THỰC TẾ] Máu của tôi vẫn còn, đối thủ đã gục. Tôi THẮNG vinh quang!");
            }
        }
    }

    private void OnHealthChanged(
        float oldValue,
        float newValue)
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (!IsOwner)
            return;

        if (healthSlider == null)
            return;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth.Value;
    }

    public float GetCurrentHealth()
    {
        return currentHealth.Value;
    }

    public MPPlayerBase GetCurrentCharacter()
    {
        return currentActiveCharacter;
    }

    //Server
    [ServerRpc]
    void SendPositionServerRpc(Vector3 pos)
    {
        UpdatePositionClientRpc(pos);
    }

    [ClientRpc]
    void UpdatePositionClientRpc(Vector3 pos)
    {
        if (IsOwner) return;
        targetPos = pos;
    }

    public void TeleportToSpawnPoint()
    {
        if (MPSpawnPoints.Instance == null)
        {
            Debug.LogError("[LỖI SPAWN] Không tìm thấy MPSpawnPoints.Instance trong Scene này!");
            return;
        }

        Vector3 spawnTargetPos = Vector3.zero;

        // Xác định điểm spawn dựa theo OwnerClientId thực tế của thực thể mạng
        if (OwnerClientId == 0) // Thực thể thuộc về Host
        {
            if (MPSpawnPoints.Instance.hostSpawn != null)
                spawnTargetPos = MPSpawnPoints.Instance.hostSpawn.position;
        }
        else // Thực thể thuộc về Client
        {
            if (MPSpawnPoints.Instance.clientSpawn != null)
                spawnTargetPos = MPSpawnPoints.Instance.clientSpawn.position;
        }

        // Thực hiện dịch chuyển
        transform.position = spawnTargetPos;

        // Cập nhật tọa độ cho nhân vật con đang active bên trong
        if (currentActiveCharacter != null)
        {
            currentActiveCharacter.transform.position = spawnTargetPos;

            // Nếu con cõng Rigidbody2D Kinematic, ép ma trận vị trí cập nhật ngay lập tức
            Rigidbody2D childRb = currentActiveCharacter.GetComponent<Rigidbody2D>();
            if (childRb != null)
            {
                childRb.velocity = Vector2.zero;
            }
        }

        Debug.Log($"[SPAWN THÀNH CÔNG] Thực thể mạng ID: {OwnerClientId} đã về đúng vị trí: {spawnTargetPos}");
    }
}