using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanPlayer : PlayerBase
{
    public Animator swordAnim;

    public GameObject swordHitbox; 

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
}
