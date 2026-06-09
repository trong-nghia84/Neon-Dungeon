using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
public class PlayerSwitchManager : MonoBehaviour
{
    public PlayerBase[] characters;
    private int currentIndex = 0;
    private PlayerBase currentActiveCharacter;

    [Header("Shared Health Settings")]
    public float sharedHealth = 100f;
    public float maxHealth = 100f;
    public bool isAllDead = false;

    [Header("UI References")]
    public Slider healthSlider; 

    public CinemachineVirtualCamera vcam;

    public static Transform CurrentPlayerTransform;
    public static PlayerSwitchManager Instance; 

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = sharedHealth;
        }
        ActivateCharacter(0);
    }

    void Update()
    {
        if (isAllDead) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 moveDir = new Vector2(moveX, moveY).normalized;

        if (currentActiveCharacter != null)
        {
            currentActiveCharacter.Move(moveDir);
        }

        if (Input.GetMouseButtonDown(0))
        {
            currentActiveCharacter.Attack();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchCharacter();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentActiveCharacter != null)
            {
                currentActiveCharacter.UseSkill1();
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (currentActiveCharacter != null)
            {
                currentActiveCharacter.UseSkill2();
            }
        }
    }

    public void TakeSharedDamage(float damage)
    {
        if (isAllDead) return;

        sharedHealth -= damage;
        UpdateHealthUI();

        if (sharedHealth <= 0)
        {
            sharedHealth = 0;
            isAllDead = true;
            if (currentActiveCharacter != null) currentActiveCharacter.Die();
            GameManager.Instance.TriggerGameOver();
        }
    }

    void SwitchCharacter()
    {
        currentIndex = (currentIndex + 1) % characters.Length;
        ActivateCharacter(currentIndex);
    }

    void ActivateCharacter(int index)
    {
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

        if (sharedHealth > maxHealth)
        {
            sharedHealth = maxHealth;
        }
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = sharedHealth;
            if (sharedHealth <= 0)
            {
                healthSlider.fillRect.gameObject.SetActive(false);
            }
            else
            {
                healthSlider.fillRect.gameObject.SetActive(true);
            }
        }
    }

}

