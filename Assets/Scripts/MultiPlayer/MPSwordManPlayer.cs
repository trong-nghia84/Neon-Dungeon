using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MPSwordsmanPlayer : MPPlayerBase
{
    public Animator swordAnim;

    public GameObject swordHitbox;

    [Header("Swordsman Skill Settings")]
    public GameObject wavePrefab;
    public Transform firePoint;

    public override void Attack()
    {
        if (isDead)
            return;

        if (anim != null)
        {
            anim.SetTrigger("IsAttack");
        }
    }

    // Animation Event
    public void EnableHitbox()
    {
        if (swordHitbox != null)
        {
            swordHitbox.SetActive(true);
        }
    }

    // Animation Event
    public void DisableHitbox()
    {
        if (swordHitbox != null)
        {
            swordHitbox.SetActive(false);
        }
    }

    protected override void ExecuteSkill1Logic()
    {
        if (anim != null)
        {
            anim.SetTrigger("IsSkill1");
        }
    }

    // Animation Event
    public void CreateSwordWave()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        if (wavePrefab == null || firePoint == null)
            return;

        GameObject wave =
            Instantiate(
                wavePrefab,
                firePoint.position,
                firePoint.rotation);

        if (!isFacingRight)
        {
            wave.transform.rotation =
                Quaternion.Euler(0, 0, 180f);
        }

        MPSwordWave waveScript =
            wave.GetComponent<MPSwordWave>();

        if (waveScript != null)
        {
            waveScript.OwnerClientId =
                switchManager.OwnerClientId;
        }

        NetworkObject netObj =
            wave.GetComponent<NetworkObject>();

        if (netObj != null)
        {
            netObj.Spawn();
        }
    }

    protected override void ExecuteSkill2Logic()
    {
        if (anim != null)
        {
            anim.SetTrigger("IsSkill2");
        }

        StartCoroutine(BlockRoutine());
    }

    private IEnumerator BlockRoutine()
    {
        isInvincible = true;

        if (anim != null)
        {
            anim.SetBool("IsSkill2", true);
        }

        yield return new WaitForSeconds(5f);

        isInvincible = false;

        if (anim != null)
        {
            anim.SetBool("IsSkill2", false);
        }
    }
}