using UnityEngine;
using Unity.Netcode;

public class MPGunnerBomb : NetworkBehaviour
{
    [Header("Bomb Settings")]
    public float delay = 2f;
    public float explosionRadius = 3f;
    public float damage = 40f;
    public float destroyDelay = 0.5f;

    [HideInInspector]
    public ulong OwnerClientId;

    private float countdown;
    private bool hasExploded = false;

    private SpriteRenderer bombSprite;
    private Rigidbody2D rb;
    private Transform bombEffectTransform;

    private void Awake()
    {
        bombSprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        bombEffectTransform = transform.Find("BombEffect");

        if (bombEffectTransform != null)
        {
            bombEffectTransform.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        countdown = delay;
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if (hasExploded)
            return;

        countdown -= Time.deltaTime;

        if (countdown <= 0f)
        {
            Explode();
        }
    }

    private void Explode()
    {
        hasExploded = true;

        if (bombSprite != null)
            bombSprite.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        if (bombEffectTransform != null)
        {
            bombEffectTransform.gameObject.SetActive(true);
        }

        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(
                transform.position,
                explosionRadius);

        foreach (Collider2D hit in colliders)
        {
            PlayerBase victim =
                hit.GetComponent<PlayerBase>();

            if (victim == null)
            {
                victim =
                    hit.GetComponentInParent<PlayerBase>();
            }

            if (victim == null)
                continue;

            MPPlayerSwitchManager victimManager =
                victim.GetComponentInParent<MPPlayerSwitchManager>();

            if (victimManager == null)
                continue;

            // Không tự nổ trúng chủ nhân
            if (victimManager.OwnerClientId == OwnerClientId)
                continue;

            victimManager.TakeDamage(damage);
        }

        Invoke(nameof(DestroyBomb), destroyDelay);
    }

    private void DestroyBomb()
    {
        NetworkObject netObj =
            GetComponent<NetworkObject>();

        if (netObj != null &&
            netObj.IsSpawned)
        {
            netObj.Despawn(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius);
    }
}