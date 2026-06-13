using UnityEngine;
using Unity.Netcode;

public class MPSwordWave : NetworkBehaviour
{
    [Header("Wave Settings")]
    public float speed = 10f;
    public float damage = 25f;
    public float lifeTime = 2f;


[HideInInspector]
    public ulong OwnerClientId;

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

        if (collision.CompareTag("Wall") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            DestroyWave();
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

        // Không tự đánh chính mình
        if (victimManager.OwnerClientId == OwnerClientId)
            return;

        victimManager.TakeDamage(damage);

        DestroyWave();
    }

    private void DestroyWave()
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


}
