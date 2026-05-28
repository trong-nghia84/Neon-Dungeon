using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWave : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 25f;
    public float lifeTime = 2f; // Tự biến mất sau 2s nếu không chạm tường

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Bay thẳng về phía trước theo hướng của nó
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Nếu chạm kẻ địch: Gây sát thương nhưng KHÔNG biến mất (Xuyên thấu)
        if (collision.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        // 2. Nếu chạm tường: Biến mất ngay lập tức
        if (collision.CompareTag("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
