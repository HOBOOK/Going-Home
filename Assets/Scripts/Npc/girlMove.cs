using Anima2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class girlMove : MonoBehaviour
{
    float standardTime = 5.0f;
    float animationTime = 0.0f;
    int randomStatus = 0;
    public float movePower = 1.0f;
    public int hp = 100;
    public GameObject attackPoint;

    bool isLeftorRight = false;
    bool isDead = false;
    bool isUnBeat = false;
    bool isAttack = false;
    bool isWait = false;
    bool isTrack = false;
    bool isJumping = false;

    int npcStatus = 0;
    float distanceBetweenTarget;
    // 공격모드,노말모드
    enum NpcMode
    {
        Normal,
        Attack,
        Event
    }

    bool isUnBeatTime;

    Rigidbody2D rigid;

    Animator animator;

    GameObject target;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        npcStatus = (int)NpcMode.Normal;
        animator.SetInteger("weaponType", 0);
        if(target==null)
        {
            target = Common.HERO();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;
        StateChecking();
    }

    private void FixedUpdate()
    {
        if (isDead)
            return;
        Jump();
        Running();
        Die();
    }

    bool DistanceChecking()
    {
        distanceBetweenTarget = Vector2.Distance(target.transform.position, transform.position);
        return distanceBetweenTarget < 5 && !status.isDead;
    }

    void ChangeBetweenAttackNormal()
    {
        if (DistanceChecking())
            npcStatus = (int)NpcMode.Attack;
        else
            npcStatus = (int)NpcMode.Normal;
    }

    void StateChecking()
    {
        switch(npcStatus)
        {
            case (int)NpcMode.Normal:
                Normal();
                break;
            case (int)NpcMode.Attack:
                AttackMode();
                break;
            case (int)NpcMode.Event:
                break;
        }
    }
    

    void Normal()
    {
        ChangeBetweenAttackNormal();
        animationTime += Time.deltaTime;
        if (animationTime >= standardTime)
        {
            StopAttack();
            animationTime = 0;
            standardTime = Random.Range(3.0f, 5.0f);
            randomStatus = Random.Range(0, 3);
            switch (randomStatus)
            {
                case 0:
                    Idle();
                    break;
                case 1:
                    Run();
                    break;
                case 2:
                    Idle();
                    break;
            }
        }
    }

    void AttackMode()
    {
        ChangeBetweenAttackNormal();
        if (distanceBetweenTarget < 0.75f && distanceBetweenTarget > 0.35f)
        {
            Idle();
            hp = 0;
        }
        
        else
        {
            //Track();
        }
            
    }

    void Idle()
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttack", false);
        animator.SetBool("isRun", false);
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void Die()
    {
        if (hp <= 0)
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            //isDead = true;
            //animator.SetTrigger("deading");
            StartCoroutine("transparentSprite");
        }
        else
            return;
    }
    void Attack()
    {
        isTrack = false;
        animator.SetBool("isMoving", false);
        animator.SetBool("isRun", false);
        if (!isAttack&&!isUnBeat&&!isWait)
        {
            StartCoroutine("Attacking");
        }
    }
    
    IEnumerator Attacking()
    {
        isAttack = true;
        isLeftorRight = target.transform.position.x < transform.position.x ? true : false;
        if (isLeftorRight)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
        animator.SetTrigger("attacking");
        animator.SetBool("isAttack", true);
        int cnt = 0;
        while(cnt<1)
        {                
            yield return new WaitForSeconds(2f);
            cnt++;
        }
        StopAttack();
        yield return null;
    }

    void Track()
    {
        isAttack = false;
        StopAttack();
        animator.SetBool("isMoving", true);
        animator.SetBool("isRun", true);
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        if(!isTrack)
            StartCoroutine("Tracking");
    }
    IEnumerator Tracking()
    {
        isTrack = true;
        int cnt = 0;
        while (cnt < 1)
        {
            isLeftorRight = target.transform.position.x < transform.position.x ? true : false;
            yield return new WaitForSeconds(1f);
            cnt++;
        }
        isTrack = false;
        yield return null;
    }

    void Run()
    {
        animator.SetBool("isMoving", true);
        animator.SetBool("isRun", false);
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        isLeftorRight = Random.Range(0, 2) == 0 ? true : false;
    }

    void Running()
    {
        if (!animator.GetBool("isMoving"))
        {
            return;
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            Vector3 moveVelocity = Vector3.zero;
            CheckHurdle();
            if (isLeftorRight)
            {
                moveVelocity = Vector3.left;
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                moveVelocity = Vector3.right;
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            transform.position += moveVelocity * (animator.GetBool("isRun") ? movePower*1.5f : movePower) * Time.deltaTime;
        }
    }
    // 장애물 낭떠러지 체크
    bool CheckHurdle()
    {
        Collider2D[] objs = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), 1.2f);
        foreach(var i in objs)
        {
            if (i.CompareTag("Wall"))
            {
                if (i.name.Contains("right"))
                {
                    if(i.transform.position.x + 0.2f<transform.position.x)
                        isLeftorRight = true;
                }
                else
                {
                    if (i.transform.position.x - 0.2f > transform.position.x)
                        isLeftorRight = false;
                }
                    
                return true;
            }
        }
        return false;
    }

    void Jump()
    {
        if (!isJumping)
            return;
        animator.SetBool("isJumping", true);
        animator.SetTrigger("jumping");
        rigid.velocity = Vector2.zero;

        Vector2 jumpVelocity = new Vector2(isLeftorRight ? -1:1, 20f);
        rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);

        isJumping = false;
    }


    IEnumerator transparentSprite()
    {
        yield return new WaitForSeconds(2);
        var sprites = GetComponentsInChildren<SpriteRenderer>();

        float alpha = 1.0f;
        while (alpha>=0)
        {
            foreach (var a in sprites)
            {
                if(!a.name.Contains("hair"))
                    a.color = new Color(1, 1, 1, alpha);
            }

            alpha -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        foreach (var a in sprites)
            a.color = new Color(1, 1, 1, 0);
        this.gameObject.SetActive(false);
        yield return null;
        
    }


    #region 물리처리
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.layer == 0 || collision.gameObject.layer == 4 || collision.gameObject.layer == 20 || collision.gameObject.layer == 22) && rigid.velocity.y < 0)
        {
            animator.SetBool("isJumping", false);
            // 낙하데미지
            //if (rigid.velocity.y < -8 && collision.gameObject.layer != 4)
            //{
            //    isUnBeatTime = true;
            //    StartCoroutine(UnBeatTime(Mathf.Abs(rigid.velocity.y * 10)));
            //}
        }
        if (collision.CompareTag("pushed"))
        {
            isJumping = true;
        }
        if (collision.CompareTag("bullet") && !collision.isTrigger &&!isDead && !isUnBeat)
        {
            isUnBeat = true;

            StopAttack();
            StartCoroutine("UnBeatTime");
            int damage = Random.Range(50, 100);
            hp = Common.looMinus(hp, damage);
            animator.SetTrigger("heating");
            Vector2 attackedVelocity = Vector2.zero;
            if (collision.gameObject.transform.position.x > transform.position.x + 0.12f)
                attackedVelocity = new Vector2(-2f, 3f);
            else
                attackedVelocity = new Vector2(2f, 3f);

            rigid.AddRelativeForce(attackedVelocity, ForceMode2D.Impulse);
        }
        //if (collision.CompareTag("attackPoint") && collision.isTrigger && !isDead && !isUnBeat)
        //{
        //    isUnBeat = true;
        //    StopAttack();
        //    StartCoroutine("UnBeatTime");
        //    int damage = Random.Range(7, 20);
        //    hp = Common.looMinus(hp, damage);
        //    animator.SetTrigger("heating");
        //    Vector2 attackedVelocity = Vector2.zero;
        //    if (collision.gameObject.transform.position.x > transform.position.x + 0.12f)
        //        attackedVelocity = new Vector2(-5f, 3f);
        //    else
        //        attackedVelocity = new Vector2(5f, 3f);

        //    rigid.AddRelativeForce(attackedVelocity, ForceMode2D.Impulse);
        //}

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Slope"))
            rigid.gravityScale = 0;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Slope"))
            rigid.gravityScale = 1;

        if (collision.gameObject.layer == 0)
        {
            GetComponent<Rigidbody2D>().simulated = true;
        }
    }
    #endregion

    IEnumerator UnBeatTime()
    {
        int count = 0;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        while (count < 1)
        {
            yield return new WaitForSeconds(0.15f);
            count++;
        }
        isUnBeat = false;
        if (hp>0)
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;

        yield return null;
    }

    IEnumerator OnAttackPoint()
    {
        int count = 0;
        attackPoint.gameObject.SetActive(true);
        while (count < 1)
        {
            yield return new WaitForSeconds(0.1f);
            count++;
        }
        attackPoint.gameObject.SetActive(false);
        yield return null;
    }

    IEnumerator WaitingForReady()
    {
        int ct = 0;
        isWait = true;
        if (isAttack)
            isAttack = false;
        while (ct < 1)
        {
            yield return new WaitForSeconds(3.0f);
            ct++;
        }
        isWait = false;
    }

    public void groundEffect()
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("KnockBack_Smoke");
        effect.transform.position = transform.position + new Vector3(0, -0.9f);
        effect.transform.rotation = status.isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        effect.SetActive(true);
    }

    public void SpeelEffect()
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Spell");
        effect.transform.position = transform.position;
        effect.SetActive(true);
    }

    public void Explosion_mEffect()
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Explosion_M");
        effect.transform.position = transform.position;
        effect.SetActive(true);

    }

    public void ShootEffect()
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Gun_Shoot");
        effect.transform.position = transform.position + new Vector3(0, 0.2f, 0) + transform.right * -1f;
        effect.transform.rotation = status.isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        effect.SetActive(true);
    }

    public void RunEffect()
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Run_Smoke");
        effect.transform.position = transform.position + new Vector3(status.isLeftorRight ? 0.3f : -0.3f, -0.7f, 0);
        effect.SetActive(true);
    }

    private void JumpEffect()
    {
        GameObject jumpEffect = ObjectPool.Instance.PopFromPool("Jump_Smoke");
        jumpEffect.transform.position = transform.position + new Vector3(0, -0.7f);
        jumpEffect.transform.rotation = status.isLeftorRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        jumpEffect.SetActive(true);
    }

    private void SwingEffect(float z)
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Sword_Cut_Medium");
        effect.transform.position = transform.position + new Vector3(status.isLeftorRight ? -1f : 1f, 0, 0);
        if (z == 0)
            z = 90;
        effect.transform.rotation = status.isLeftorRight ? Quaternion.Euler(0, 90, z) : Quaternion.Euler(0, 270, z);
        effect.SetActive(true);
    }

    public void SwingEffect2()
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Sword_Thrust_Small");
        effect.transform.position = transform.position + new Vector3(status.isLeftorRight ? -0.5f : 0.5f, -0.5f, 0);
        effect.transform.rotation = status.isLeftorRight ? Quaternion.Euler(120, 225, 90) : Quaternion.Euler(120, 45, 90);
        effect.SetActive(true);

    }

    private void SkillEffect()
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("MagicCasting");
        effect.transform.position = transform.position + new Vector3(status.isLeftorRight ? -0.7f : 0.7f, 0, 0);
        effect.SetActive(true);
    }


    void StopAttack()
    {
        animator.SetBool("isAttack", false);
        if(!isWait)
            StartCoroutine("WaitingForReady");
    }

    #region 사운드 매니저

    public void SoundUpdate()
    {
        Sound_HeartBeat();
    }
    public void Sound_HeartBeat()
    {
        if (status.hp < status.hpFull * 0.3f)
        {
            SoundManager.instance.PlaySingleLoop(AudioClipManager.instance.heartBeat);
        }
        else
        {
            SoundManager.instance.StopSingleLoop();
        }
    }
    public void Sound_ReloadPistol()
    {
        SoundManager.instance.PlaySingle(AudioClipManager.instance.reloadPistol);
    }
    public void Sound_Damage()
    {
        SoundManager.instance.RandomizeSfx(AudioClipManager.instance.damage1, AudioClipManager.instance.damage2);
    }
    public void Sound_Pickup()
    {
        SoundManager.instance.RandomizeSfx(AudioClipManager.instance.pickup);
    }

    public void Sound_Equip()
    {
        SoundManager.instance.RandomizeSfx(AudioClipManager.instance.equip);
    }

    public void Sound_Dead()
    {
        SoundManager.instance.RandomizeSfx(AudioClipManager.instance.dead);
    }

    public void Sound_Kinfe()
    {
        SoundManager.instance.RandomizeSfx(AudioClipManager.instance.knife1, AudioClipManager.instance.knife2);
    }
    public void Sound_Jump()
    {
        SoundManager.instance.PlaySingle(AudioClipManager.instance.jump);
    }

    public void Sound_Punch()
    {
        SoundManager.instance.PlaySingle(AudioClipManager.instance.punchHit);
    }

    public void Sound_Sword()
    {
        SoundManager.instance.PlaySingle(AudioClipManager.instance.swingSword);
    }
    public void Sound_Shoot()
    {
        SoundManager.instance.PlaySingle(AudioClipManager.instance.shootPistol);
    }
    #endregion

}
