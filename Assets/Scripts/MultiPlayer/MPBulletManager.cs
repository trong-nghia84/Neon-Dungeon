using UnityEngine;
using Unity.Netcode;

public class MPBulletManager : NetworkBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;
    public float damage = 15f;
    public float lifeTime = 2f;


[HideInInspector]
    public ulong OwnerClientId;

    [Header("Effects")]
    public GameObject impactEffect;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer)
            return;

        if (collision.CompareTag("Wall"))
        {
            DestroyBullet();
            return;
        }

        PlayerBase victim =
            collision.GetComponent<PlayerBase>();

        if (victim == null)
        {
            victim =
                collision.GetComponentInParent<PlayerBase>();
        }

        if (victim == null)
            return;

        MPPlayerSwitchManager victimManager =
            victim.GetComponentInParent<MPPlayerSwitchManager>();

        if (victimManager == null)
            return;

        if (victimManager.OwnerClientId == OwnerClientId)
            return;

        victimManager.TakeDamage(damage);

        DestroyBullet();
    }

    private void DestroyBullet()
    {
        if (impactEffect != null)
        {
            Instantiate(
                impactEffect,
                transform.position,
                Quaternion.identity);
        }

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


}
