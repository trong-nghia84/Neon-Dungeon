using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwitchManager : MonoBehaviour
{
    public PlayerBase[] characters;
    private int currentIndex = 0;
    private PlayerBase currentActiveCharacter;

    [Header("Shared Health Settings")]
    public float sharedHealth = 100f;
    public float maxHealth = 100f;
    public bool isAllDead = false;

    public CinemachineVirtualCamera vcam;

    public static Transform CurrentPlayerTransform;
    void Start()
    {
        ActivateCharacter(0);
    }

    void Update()
    {
        // NẾU ĐÃ CHẾT THÌ KHÔNG LÀM GÌ CẢ
        if (isAllDead) return;

        // 1. Xử lý di chuyển 8 hướng
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 moveDir = new Vector2(moveX, moveY).normalized;

        if (currentActiveCharacter != null)
        {
            currentActiveCharacter.Move(moveDir);
        }

        // 2. Xử lý tấn công
        if (Input.GetMouseButtonDown(0))
        {
            currentActiveCharacter.Attack();
        }

        // 3. Xử lý đổi nhân vật (Chỉ cho phép đổi khi còn sống)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchCharacter();
        }
    }

    // Hàm dùng chung để trừ máu từ bất kỳ nhân vật nào
    public void TakeSharedDamage(float damage)
    {
        if (isAllDead) return;

        sharedHealth -= damage;
        Debug.Log("Máu chung còn lại: " + sharedHealth);

        if (sharedHealth <= 0)
        {
            sharedHealth = 0;
            isAllDead = true;
            // Ép nhân vật hiện tại thực hiện animation chết
            if (currentActiveCharacter != null) currentActiveCharacter.Die();
        }
    }

    void SwitchCharacter()
    {
        currentIndex = (currentIndex + 1) % characters.Length;
        ActivateCharacter(currentIndex);
    }

    void ActivateCharacter(int index)
    {
        // Lưu vị trí trước khi chuyển
        Vector3 lastPosition = currentActiveCharacter != null ? currentActiveCharacter.transform.position : transform.position;

        for (int i = 0; i < characters.Length; i++)
        {
            bool isActive = (i == index);
            characters[i].gameObject.SetActive(isActive);

            if (isActive)
            {
                characters[i].transform.position = lastPosition;
                currentActiveCharacter = characters[i];

                CurrentPlayerTransform = characters[i].transform;

                if (vcam != null)
                {
                    vcam.Follow = characters[i].transform;
                }
            }
        }
    }

    public void HealSharedHealth(float amount)
    {
        if (isAllDead) return;

        sharedHealth += amount;

        // Giả sử máu tối đa của bạn là 100 (hãy thay bằng biến maxHealth nếu có)
        if (sharedHealth > maxHealth)
        {
            sharedHealth = maxHealth;
        }

        Debug.Log("Máu hiện tại sau khi hồi: " + sharedHealth);
    }

}

