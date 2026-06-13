using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 15f;
    public float lifeTime = 2f;

    public enum OwnerType { Player, Enemy }
    [Header("Cài đặt phe phái")]
    public OwnerType owner;

    [Header("Effects")]
    public GameObject impactEffect;

    private Rigidbody2D rb;
    private bool isLaunched = false; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector2 direction)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        rb.velocity = direction * speed;
        isLaunched = true;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        if (!isLaunched)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Traps")
        {
            return; 
        }

        if (collision.CompareTag("Wall") || collision.GetComponent<UnityEngine.Tilemaps.TilemapCollider2D>() != null)
        {
            HandleImpact();
            return;
        }

        if (owner == OwnerType.Enemy && collision.CompareTag("Player"))
        {
            PlayerBase player = collision.GetComponent<PlayerBase>();
            if (player == null) player = collision.GetComponentInParent<PlayerBase>();

            if (player != null)
            {
              
                player.TakeDamage(damage);
            }

            HandleImpact();
        }

        if (owner == OwnerType.Player && collision.CompareTag("Enemy"))
        {
            EnemyBase enemy = collision.GetComponent<EnemyBase>();

           
            float finalDamage = damage;

            if (PlayerSwitchManager.CurrentPlayerTransform != null)
            {
                PlayerBase activePlayer = PlayerSwitchManager.CurrentPlayerTransform.GetComponent<PlayerBase>();
                if (activePlayer != null)
                {
                   
                    finalDamage = damage * activePlayer.GetDamageMultiplier();
                }
            }

            if (enemy != null)
            {
                enemy.TakeDamage(finalDamage);
                Debug.Log($"Đạn Player bắn trúng Enemy gây: {finalDamage} sát thương.");
            }

            HandleImpact();
        }
    }

    private void HandleImpact()
    {
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}