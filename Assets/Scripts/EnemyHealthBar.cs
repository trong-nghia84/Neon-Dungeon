using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Vị trí thanh máu so với đầu kẻ địch

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
    }

    void LateUpdate()
    {
        // Giữ vị trí luôn ở trên đầu Enemy
        transform.position = transform.parent.position + offset;

        // QUAN TRỌNG: Giữ thanh máu không bị lật khi Enemy xoay (Flip)
        transform.rotation = Quaternion.identity;
    }
}