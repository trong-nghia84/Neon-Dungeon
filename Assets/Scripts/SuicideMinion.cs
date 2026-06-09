using System.Collections;
using UnityEngine;

public class SuicideMinion : EnemyBase
{
    [Header("Cấu hình Nổ")]
    public float explosionDamage = 30f;
    public float explosionRadius = 2f;
    public float timeToExplode = 1f; 

    [Header("Hiệu ứng")]
    public GameObject explosionVFXPrefab; 
    public GameObject warningAreaVFX;       

    private bool isPreparingToExplode = false;
    private SpriteRenderer sprite;

    protected override void Awake()
    {
        base.Awake();
        sprite = GetComponent<SpriteRenderer>();
        if (warningAreaVFX != null) warningAreaVFX.SetActive(false);
    }
    protected override void Update()
    {
        base.Update();

        if (isDead || player == null || isPreparingToExplode)
        {
            StopMovement();
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        if (direction.x > 0 && transform.localScale.x < 0) Flip();
        else if (direction.x < 0 && transform.localScale.x > 0) Flip();
    }

    private void Flip()
    {
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead || isPreparingToExplode) return;

        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ExplosionRoutine());
        }
    }

    private IEnumerator ExplosionRoutine()
    {
        isPreparingToExplode = true;
        StopMovement(); 

        Debug.Log(gameObject.name + " chuẩn bị kích nổ!");

        if (warningAreaVFX != null) warningAreaVFX.SetActive(true);

        if (sprite != null)
        {
            for (int i = 0; i < 3; i++)
            {
                sprite.color = Color.red; 
                yield return new WaitForSeconds(timeToExplode / 6f);
                sprite.color = Color.white; 
                yield return new WaitForSeconds(timeToExplode / 6f);
            }
        }
        else
        {
            yield return new WaitForSeconds(timeToExplode);
        }

        ExecuteFinalExplosion();
    }

    private void ExecuteFinalExplosion()
    {
        isDead = true; 

        if (explosionVFXPrefab != null)
        {
            GameObject vfx = Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);

            Destroy(vfx, 0.5f);
        }

        SpriteRenderer minionSprite = GetComponent<SpriteRenderer>();
        if (minionSprite != null) minionSprite.enabled = false;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D obj in colliders)
        {
            if (obj.CompareTag("Player"))
            {
                PlayerBase p = obj.GetComponent<PlayerBase>();
                if (p == null) p = obj.GetComponentInParent<PlayerBase>();

                if (p != null)
                {
                    p.TakeDamage(explosionDamage);
                    Debug.Log("Minion nổ trúng Player!");
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}