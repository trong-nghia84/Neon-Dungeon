using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBarUI : MonoBehaviour
{
    public Slider hpSlider;          
    public TextMeshProUGUI nameText; 

    private BossController activeBoss;


    void Start()
    {
        gameObject.SetActive(false);
    }
    public void SetupBossHealthBar(BossController boss, string bossName)
    {
        activeBoss = boss;
        if (nameText != null) nameText.text = bossName;

        gameObject.SetActive(true); 
        UpdateHealthBar();
    }

    void Update()
    {
        if (activeBoss != null)
        {
            UpdateHealthBar();

            if (activeBoss.isDead)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void UpdateHealthBar()
    {
        if (activeBoss != null && hpSlider != null)
        {
            float hpRatio = activeBoss.GetHealthNormalized();
            hpSlider.value = hpRatio;
        }
    }
}