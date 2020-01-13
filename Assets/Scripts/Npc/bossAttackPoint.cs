using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossAttackPoint : MonoBehaviour
{
    public bool isSkill = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")&&isSkill&&!status.isDead)
        {
            collision.GetComponentInParent<move>().Knockback(15, 25);
            collision.GetComponentInParent<move>().isDown = true;
            collision.GetComponentInParent<move>().moveAnimator.SetTrigger("falling");
            isSkill = false;
        }
    }
}
