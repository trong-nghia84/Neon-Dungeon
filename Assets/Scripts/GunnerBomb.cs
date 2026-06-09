using UnityEngine;

public class GunnerBomb : MonoBehaviour
{
    [Header("Cấu hình Bom")]
    public float delay = 2f;            
    public float explosionRadius = 3f;   
    public float damage = 40f;           
    public float destroyDelay = 0.5f;    

    private float countdown;
    private bool hasExploded = false;

 
    private SpriteRenderer bombSprite;
    private Rigidbody2D rb;
    private Transform bombEffectTransform;

    void Awake()
    {
      
        bombSprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        bombEffectTransform = transform.Find("BombEffect");

        if (bombEffectTransform != null)
        {
            bombEffectTransform.gameObject.SetActive(false);
        }
        else
        {
        }
    }

    void Start()
    {
        countdown = delay;
    }

    void Update()
    {
        if (hasExploded) return;

        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            Explode();
        }
    }

    void Explode()
    {
        hasExploded = true;

        if (bombSprite != null) bombSprite.enabled = false;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        if (bombEffectTransform != null)
        {
            bombEffectTransform.gameObject.SetActive(true);
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag("Enemy"))
            {
                EnemyBase enemy = nearbyObject.GetComponent<EnemyBase>();
                if (enemy == null) enemy = nearbyObject.GetComponentInParent<EnemyBase>();

                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        Destroy(gameObject, destroyDelay);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}