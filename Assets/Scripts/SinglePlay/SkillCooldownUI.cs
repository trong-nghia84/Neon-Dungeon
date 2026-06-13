using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    [Header("Cấu hình Ô Skill")]
    [Tooltip("Điền số 1 cho ô Skill 1, số 2 cho ô Skill 2")]
    public int skillNumber = 1;

    [Header("UI References")]
    public Image skillIconImage; 
    public Image cooldownImage;  

    void Start()
    {
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f;
        }
    }

    void Update()
    {
        if (PlayerSwitchManager.CurrentPlayerTransform != null)
        {
            PlayerBase activePlayer = PlayerSwitchManager.CurrentPlayerTransform.GetComponent<PlayerBase>();

            if (activePlayer != null)
            {
                
                if (skillIconImage != null)
                {
                    Sprite targetIcon = (skillNumber == 1) ? activePlayer.skill1Icon : activePlayer.skill2Icon;

                    if (targetIcon != null)
                    {
                        skillIconImage.sprite = targetIcon;
                    }
                }

               
                if (cooldownImage != null)
                {
                    float cooldownPercentage = activePlayer.GetCooldownNormalized(skillNumber);

                    cooldownImage.fillAmount = cooldownPercentage;
                }
            }
        }
    }
}