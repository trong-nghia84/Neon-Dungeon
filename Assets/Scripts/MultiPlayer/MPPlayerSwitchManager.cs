using Cinemachine;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
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

    [Header("Camera")]
    public CinemachineVirtualCamera vcam;

    public static MPPlayerSwitchManager LocalPlayer;

    public override void OnNetworkSpawn()
    {
        activeCharacterIndex.OnValueChanged += OnCharacterChanged;
        currentHealth.OnValueChanged += OnHealthChanged;

        if (IsServer)
        {
            currentHealth.Value = maxHealth;
            activeCharacterIndex.Value = 0;
        }

        // CHỈ MÁY SỞ HỮU MỚI ĐƯỢC TỰ ĐỊNH VỊ VỊ TRÍ SPAWN CỦA MÌNH
        if (IsOwner && MPSpawnPoints.Instance != null)
        {
            Vector3 spawnTargetPos = Vector3.zero;

            if (NetworkManager.Singleton.LocalClientId == 0) // Máy Host
            {
                spawnTargetPos = MPSpawnPoints.Instance.hostSpawn.position;
            }
            else // Máy Client
            {
                spawnTargetPos = MPSpawnPoints.Instance.clientSpawn.position;
            }

            // LỆNH DỊCH CHUYỂN TỐI CAO: Gán trực tiếp transform vì Rigidbody đã là Kinematic
            transform.position = spawnTargetPos;
        }

        // Bật nhân vật con lên
        ActivateCharacter(activeCharacterIndex.Value);
        UpdateHealthUI();
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
        if (currentActiveCharacter != null)
        {
            currentActiveCharacter.Die();
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
}