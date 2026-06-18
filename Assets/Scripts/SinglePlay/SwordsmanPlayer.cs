using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanPlayer : PlayerBase
{
    public Animator swordAnim;

    public GameObject swordHitbox;

    [Header("Swordsman Skill Settings")]
    public GameObject wavePrefab;
    public Transform firePoint;

    public override void Attack()
    {
        if (isDead) return;
        anim.SetTrigger("IsAttack");
    }

    public void EnableHitbox()
    {
        if (swordHitbox != null)
            swordHitbox.SetActive(true);
    }

    public void DisableHitbox()
    {
        if (swordHitbox != null)
            swordHitbox.SetActive(false);
    }

    protected override void ExecuteSkill1Logic()
    {
        anim.SetTrigger("IsSkill1");
    }

    public void CreateSwordWave()
    {
        if (wavePrefab != null && firePoint != null)
        {
            GameObject wave = Instantiate(wavePrefab, firePoint.position, firePoint.rotation);

            if (!isFacingRight)
            {
                wave.transform.rotation = Quaternion.Euler(0, 0, 180f);
            }
        }
    }

    protected override void ExecuteSkill2Logic()
    {
        anim.SetTrigger("IsSkill2");
        StartCoroutine(BlockRoutine());
    }

    private IEnumerator BlockRoutine()
    {

        isInvincible = true;
        moveSpeed = 3f;
        if (anim != null) anim.SetBool("IsSkill2", true);
        
        yield return new WaitForSeconds(5f);

        isInvincible = false;
        moveSpeed = 5f;

        if (anim != null) anim.SetBool("IsSkill2", false);
        

    }
}
