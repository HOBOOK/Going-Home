using Anima2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class bossMove : MonoBehaviour
{
    public string Name = "";
    float standardTime = 5.0f;
    float animationTime = 0.0f;
    float skyTime = 0.0f;
    float attackMinRange;
    float attackMaxRange;
    float attackChangeRange = 2;
    bool isRangeMode = false;
    public float detectionRange=20;
    int randomStatus = 0;
    public int comboCount = 3;
    public float movePower = 1.0f;
    public int hp = 100;
    public int maxHp;
    public float attack = 10;

    public GameObject weaponPoint;
    public GameObject attackPoint;
    public List<GameObject> dropItem = new List<GameObject>();

    RaycastHit2D checkJumpHit2D;
    RaycastHit2D checkTurnHit2D;
    Collider2D[] collider2dObjs;

    public bool isNeutrality;
    public bool isHold = false;
    Vector3 firstPos = Vector3.zero;
    public bool isLeftorRight = false;
    public bool isDead = false;
    public bool isUnBeat = false;
    public bool isAttack = false;
    public bool isWait = false;
    public bool isTrack = false;
    public bool isJumping = false;
    public bool isDefence = false;
    public bool isStun = false;
    public bool isStunning = false;
    public bool isClimb = false;
    public bool isClimbing = false;
    public bool isAir = false;
    public bool isSkill = false;
    bool isChangingWeapon = false;
    public List<string> initChats = new List<string>();
    bool isAirborne = false;
    float firstDetectionRange;
    float attackDetectionRange = 30;
    float firstMovePower;
    float attackMovePower;

    Vector3 wallPos;
    Vector3 pos;

    public NpcMode npcMode;
    float distanceBetweenTarget;

    // 공격모드,노말모드
    public enum NpcMode
    {
        Normal,
        Attack,
        Event
    }
    public enum SkillType
    {
        Near1,
        Near2,
        Range1,
        Range2
    }
    public int skillGuage = 0;
    public SkillType skillType;

    public Common.WeaponType NpcWeapon;
    public Common.WeaponType[] Weapons = new Common.WeaponType[2];

    Rigidbody2D rigid;

    Animator animator;
    Animator faceAnimator;

    public GameObject target;

    void Start()
    {
        this.name = Name;
        maxHp = hp;
        firstPos = this.transform.position;
        firstMovePower = movePower;
        attackMovePower = firstMovePower * 1.5f;
        firstDetectionRange = detectionRange;
        animator = GetComponent<Animator>();
        faceAnimator = GetComponentInChildren<faceOff>().GetFaceAnimator();
        rigid = GetComponent<Rigidbody2D>();
        npcMode = NpcMode.Normal;
        attackPoint.GetComponent<SpriteRenderer>().enabled = false;
        InitSpriteLayer();

        if (!target)
            target = Common.HERO();

        SetAttackAnimation();
        EquipWeapon(NpcWeapon);
        OpenHpUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;
        StateChecking();
        SkillUpdate();
        Die();
    }

    private void FixedUpdate()
    {
        if (isDead)
            return;
        Jump();
        SkyCheck();
        Climb();
        Running();
    }

    void InitSpriteLayer()
    {
        foreach(var i in GetComponentsInChildren<SpriteRenderer>())
        {
            i.sortingOrder += (int)transform.position.z*10;
        }
    }
    bool HeightChecking(float Y)
    {
        if (target.transform.position.y > transform.position.y + Y)
            return false;
        return true;
    }
    IEnumerator FocusingTarget()
    {
        Debugging.Log(" 포커싱 시작 >> " +this.name);
        Common.GetCameraObject().GetComponent<FollowCamera>().ChangeTarget(this.gameObject);
        yield return new WaitForSeconds(2.0f);
        Common.GetCameraObject().GetComponent<FollowCamera>().ChangeTarget(Common.HERO());
        yield return null;
    }
    void ChangeWeaponCheck()
    {
        if (Weapons.Length > 1)
        {
            if (npcMode == NpcMode.Attack && Weapons[1] != Common.WeaponType.no && !isSkill && !isStunning && !isWait && !isAttack && !isAir)
            {
                if (isRangeMode)
                {
                    if (distanceBetweenTarget < attackChangeRange)
                    {
                        animator.SetBool("isThrow", false);
                        EquipWeapon(Weapons[0]);
                    }
                    isRangeMode = false;
                }
                else
                {
                    if (distanceBetweenTarget > attackChangeRange + 1.5f)
                    {
                        animator.SetBool("isThrow", true);
                        EquipWeapon(Weapons[1]);
                    }
                    isRangeMode = true;
                }
            }
        }
  
    }

    void DistanceChecking()
    {
        distanceBetweenTarget = Vector2.Distance(Common.GetBottomPosition(target.transform), Common.GetBottomPosition(transform));
        ChangeWeaponCheck();
    }
    void RemoveWeapon()
    {
        if (weaponPoint.transform.childCount > 0)
        {
            Destroy(weaponPoint.transform.GetChild(0).gameObject);
        }
    }

    void EquipWeapon(Common.WeaponType changeWeapon)
    {
        if (NpcWeapon != changeWeapon)
        {
            isChangingWeapon = true;
            animator.SetTrigger("changingWeapon");
            NpcWeapon = changeWeapon;
            RemoveWeapon();
            if (changeWeapon == Common.WeaponType.no)
            {
            }
            else
            {
                if (weaponPoint != null)
                {
                    GameObject weapon = Instantiate(Common.GetPrefabDatabase().GetComponent<PrefabsDatabaseManager>().GetWeapon((int)NpcWeapon), weaponPoint.transform) as GameObject;
                    weapon.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Default";
                    weapon.GetComponentInChildren<SpriteRenderer>().sortingOrder = 3 + ((int)transform.position.z * 10);
                    weapon.GetComponent<Rigidbody2D>().isKinematic = true;
                    weapon.GetComponent<PolygonCollider2D>().enabled = false;
                    weapon.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                    weapon.GetComponent<Rigidbody2D>().angularVelocity = 0;
                    if (weapon.GetComponent<BoxCollider2D>())
                        weapon.GetComponent<BoxCollider2D>().enabled = false;
                    weapon.transform.localPosition = new Vector3(0, 0, 0);
                    weapon.transform.rotation = new Quaternion(0, 0, 0, 0);
                    weapon.SetActive(true);
                }
            }
            SetAttackAnimation();
            isChangingWeapon = false;
        }
    }

    void DroppingItem()
    {
        if(dropItem.Count>0)
        {
            for(int i = 0; i < dropItem.Count; i++)
            {
                GameObject item = Instantiate(dropItem[i], GameObject.Find("dropItems").transform) as GameObject;
                item.transform.position = transform.position;
                if(i%2==0)
                {
                    item.GetComponent<Rigidbody2D>().AddForce(new Vector3(i*1, 7,10), ForceMode2D.Impulse);
                }
                else
                {
                    item.GetComponent<Rigidbody2D>().AddForce(new Vector3(i * -1, 7,10), ForceMode2D.Impulse);
                }
            }
        }
    }

    void ChangeNpcMode()
    {
        DistanceChecking();
        if (isNeutrality)
            return;
        if (npcMode != NpcMode.Event)
        {
            if(target.activeSelf)
            {
                if (distanceBetweenTarget > detectionRange)
                {
                    npcMode = NpcMode.Normal;
                    detectionRange = firstDetectionRange;
                    movePower = firstMovePower;
                    EquipWeapon(Common.WeaponType.no);

                }
                else
                {
                    if (npcMode == NpcMode.Normal)
                    {
                        detectionRange = attackDetectionRange;
                        movePower = attackMovePower;
                        if (initChats.Count > 0)
                            Common.Chat(initChats[UnityEngine.Random.Range(0, initChats.Count)], transform);
                        EquipWeapon(Weapons[0]);
                        Common.hitTargetName = this.Name;
                        npcMode = NpcMode.Attack;

                        StartCoroutine("FocusingTarget");

                    }
                }
            }
            else
            {
                npcMode = NpcMode.Normal;
                detectionRange = firstDetectionRange;
                movePower = firstMovePower;
                EquipWeapon(Common.WeaponType.no);
            }

        }
    }

    void StateChecking()
    {
        switch(npcMode)
        {
            case NpcMode.Normal:
                Normal();
                break;
            case NpcMode.Attack:
                AttackMode();
                break;
            case NpcMode.Event:
                break;
        }
    }

    public void Chat(string chat)
    {
        Common.Chat(chat, transform);
    }
    public void HitEffect(Collider2D collider)
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Hit_white_Small");
        effect.transform.position = transform.position+ new Vector3(UnityEngine.Random.Range(-0.2f,0.2f),0.2f,0);
        effect.SetActive(true);
    }
    private void SwordHitEffect(Collider2D collider)
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Sword_Hit");
        effect.transform.position = new Vector3(transform.position.x, collider.transform.position.y,0) ;
        effect.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(-0.2f, 0.2f), 0, UnityEngine.Random.Range(-10, 10));
        effect.SetActive(true);
    }


    public void GuardEffect(Collider2D collider)
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Guard_hit");
        effect.transform.position = transform.position + new Vector3(UnityEngine.Random.Range(-0.2f, 0.2f), 0.2f, 0);
        effect.SetActive(true);
    }

    public void ShootEffect()
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Gun_Shoot");
        effect.transform.position = transform.position + new Vector3(0, 0.2f, 0) + transform.right * -1f;
        effect.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        effect.SetActive(true);
    }
    public void SpellEffect()
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

    public void NpcShoot()
    {
        GameObject bullet = ObjectPool.Instance.PopFromPool("Bullet");
        ShootEffect();
        bullet.GetComponent<bulletController>().Target = target.transform;
        bullet.transform.position = transform.position + new Vector3(0, 0.1f) + transform.right * -1f;
        bullet.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        bullet.layer = LayerMask.NameToLayer("NpcBullet");
        bullet.SetActive(true);
    }

    public void NpcThrowing()
    {
        GameObject knife = ObjectPool.Instance.PopFromPool("Knife");
        knife.GetComponent<bulletController>().Target = target.transform;
        knife.transform.position = transform.position + new Vector3(0, 0.1f) + transform.right * -1f;
        knife.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        knife.layer = LayerMask.NameToLayer("NpcBullet");
        knife.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("NpcBullet");
        knife.SetActive(true);
    }



    void Normal()
    {
        ChangeNpcMode();
        animationTime += Time.deltaTime;
        if (animationTime >= standardTime)
        {
            StopAttack();
            animationTime = 0;
            standardTime = UnityEngine.Random.Range(3.0f, 5.0f);
            randomStatus = UnityEngine.Random.Range(0, 3);
            if (isHold)
                randomStatus = 3;
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
                case 3:
                    Hold();
                    break;
            }
        }
    }
    void SetAttackAnimation()
    {
        switch (NpcWeapon)
        {
            case Common.WeaponType.no:
                animator.SetInteger("weaponType", 0);
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(3, 2);
                attackMinRange = 0;
                attackMaxRange = UnityEngine.Random.Range(2, 2.5f);
                break;
            case Common.WeaponType.gun:
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(3, 2);
                animator.SetInteger("weaponType", 1);
                attackMinRange = attackChangeRange+ UnityEngine.Random.Range(1, 2);
                attackMaxRange = UnityEngine.Random.Range(7, 9);

                break;

            case Common.WeaponType.sword:
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(3, 2);
                animator.SetInteger("weaponType", 2);
                attackMinRange = 0;
                attackMaxRange = UnityEngine.Random.Range(2, 2.5f);
                break;

            case Common.WeaponType.knife:
                attackPoint.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(3, 2);
                if (animator.GetBool("isThrow"))
                {
                    attackMinRange = attackChangeRange + UnityEngine.Random.Range(1, 2);
                    attackMaxRange = UnityEngine.Random.Range(5, 7);
                }
                else
                {
                    attackMinRange = 0;
                    attackMaxRange = UnityEngine.Random.Range(2, 2.5f);
                }

                animator.SetInteger("weaponType", 3);
                break;
        }
    }

    void AttackMode()
    {
        DistanceChecking();
        if (distanceBetweenTarget < attackMaxRange && distanceBetweenTarget >= attackMinRange && HeightChecking(3)&&!status.isDead&&target.activeSelf)
        {
            Attack();
        }
        else
        {
            if (!isAttack&&!isUnBeat&&!isStunning&&!isClimb&&!isWait&&!isAirborne&&!isSkill&&!isChangingWeapon&&!status.isDead)
            {
                ChangeNpcMode();
                Track();
            }
        }
    }

    void Defence(Collider2D collision)
    {
        if (isNeutrality)
            isNeutrality = false;
        isUnBeat = true;
        isDefence = true;
        RedirectCharacter();
        if (collision.GetComponentInParent<move>() != null)
            collision.GetComponentInParent<move>().Knockback(5, 1);
        if (!animator.GetBool("isAttack"))
        {
            animator.SetTrigger("defencing");
            faceAnimator.SetTrigger("Do");
            Knockback(1, 1, collision);
        }
        DamageFontShow("Defence", false , true);
        
        StartCoroutine(UnBeatTime(0));

        isWait = false;
        animator.SetBool("isWait", false);
    }
    void Knockback(float kx, float ky, Collider2D collision = null)
    {
        Vector2 attackedVelocity = Vector2.zero;

        if (collision != null&&!isDead)
        {

            if (collision.transform.position.x > transform.position.x)
            {
                if (this.transform.rotation.y == 0)
                    animator.SetBool("isFrontHit", false);
                else
                    animator.SetBool("isFrontHit", true);

                attackedVelocity = new Vector2(-kx, ky);
            }
            else
            {
                if (this.transform.rotation.y == 0)
                    animator.SetBool("isFrontHit", true);
                else
                    animator.SetBool("isFrontHit", false);

                attackedVelocity = new Vector2(kx, ky);
            }

        }
        else
        {
            if (this.transform.rotation.y == 0 && status.isLeftorRight)
                animator.SetBool("isFrontHit", false);
            else
                animator.SetBool("isFrontHit", true);

            attackedVelocity = target.transform.position.x > transform.position.x ? new Vector2(-kx, ky) : new Vector2(kx, ky);
        }
        rigid.AddForce(attackedVelocity, ForceMode2D.Impulse);
    }

    void Hitted(Collider2D collision, int dam, float kx, float ky)
    {
        if(isNeutrality)
            isNeutrality = false;
        isUnBeat = true;
        DamageFontShow(dam.ToString(), dam > status.maxAttack * 0.95f ? true : false);
        Common.isHitShake = true;

        if(isStunning)
        {
            Knockback(0, ky, collision);
        }
        else if(!animator.GetBool("isAttack")&&!isSkill)
        {
            Knockback(kx, ky, collision);
            animator.SetTrigger("heating");
        }
        faceAnimator.SetTrigger("Hit");
        hp = Common.looMinus(hp, dam);
        StopAttack();
        StartCoroutine(UnBeatTime(dam));
    }

    public void Stunned()
    {
        if(isStun&&!isDead)
        {
            StartCoroutine("Stunning");
        }
    }

    IEnumerator Stunning()
    {
        isStunning = true;
        Debugging.Log(name + " >> 스턴상태");
        Knockback(10, 1);
        animator.SetTrigger("stunning");
        animator.SetBool("isStun", true);
        faceAnimator.SetTrigger("Stun");
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        while (isStunning)
        {
            yield return new WaitForSeconds(3.0f);
            isStun = false;
            isStunning = false;
        }
        Debugging.Log(name + " >> 스턴상태끝");
        animator.SetBool("isStun", false);
        yield return null;
    }

    void Idle()
    {
        animator.SetBool("isMoving", false);
        animator.SetBool("isAttack", false);
        animator.SetBool("isRun", false);
        rigid.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }

    void Die()
    {
        if (hp <= 0&&!isDead)
        {
            Common.hitTargetName = "";
            StopAllCoroutines();
            DroppingItem();
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigid.gravityScale = 1;
            Knockback(12, 23);
            isDead = true;
            animator.SetBool("isStun", false);
            animator.SetBool("isWait", false);
            faceAnimator.SetBool("isDead", true);
            animator.SetTrigger("deading");
            ColorChangeSprites(new Color(1, 1, 1));
            StartCoroutine("TransparentSprite");
        }
        else
            return;
    }
    void Attack()
    {
        isTrack = false;
        animator.SetBool("isMoving", false);
        animator.SetBool("isRun", false);

        if (!isAttack&&!isUnBeat&&!isWait&&!isStunning&&!isAirborne&&!isSkill&&!isChangingWeapon)
        {
            StartCoroutine("Attacking");
        }
        else if(!isAttack&&!isSkill)
        {
            if (attackPoint)
                attackPoint.SetActive(false);
        }
    }
    void RedirectCharacter()
    {
        if (target.activeSelf)
        {
            isLeftorRight = target.transform.position.x < transform.position.x ? true : false;
            transform.rotation = isLeftorRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        }
    }
    
    IEnumerator Attacking()
    {
        if(!isSkill && attackPoint.GetComponent<bossAttackPoint>().isSkill)
        {
            attackPoint.GetComponent<bossAttackPoint>().isSkill = false;
        }
        isAttack = true;
        RedirectCharacter();
        animator.SetTrigger("attacking");
        faceAnimator.SetTrigger("Do");
        animator.SetBool("isAttack", true);
        int cnt = 0;
        while(cnt< comboCount&&!isStunning)
        {
            animator.SetInteger("combo", cnt);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            cnt++;
        }
        StopAttack();
        yield return null;
    }

    void Track()
    {
        ChangeNpcMode();
        animator.SetBool("isMoving", true);
        animator.SetBool("isRun", true);
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        if(!isTrack)
            StartCoroutine(Tracking(target.transform.position));
    }
    IEnumerator Tracking(Vector3 tPos)
    {
        isTrack = true;
        int cnt = 0;
        while (cnt < 1)
        {
            isLeftorRight = tPos.x > transform.position.x ? false : true;
            yield return new WaitForSeconds(1f);
            if (!HeightChecking(2)&&distanceBetweenTarget< 4)
                isJumping = true;
            cnt++;
        }
        isTrack = false;
        yield return null;
    }

    void Hold()
    {   
        if (Vector3.Distance(firstPos, transform.position) > 2)
        {
            ChangeNpcMode();
            animator.SetBool("isMoving", true);
            animator.SetBool("isRun", true);
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            if (!isTrack)
                StartCoroutine(Tracking(firstPos));
        }
        else
        {
            Idle();
        }
    }

    void Run()
    {
        animator.SetBool("isMoving", true);
        animator.SetBool("isRun", false);
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        isLeftorRight = UnityEngine.Random.Range(0, 2) == 0 ? true : false;
    }

    void Running()
    {
        ChangeNpcMode();

        if (animator.GetBool("isMoving")&&!isStunning&&!isClimb)
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            Vector3 moveVelocity = Vector3.zero;
            CheckHurdle();
            if(!isSkill)
            {
                RedirectCharacter();
                moveVelocity = isLeftorRight ? Vector3.left : Vector3.right;
            }
            transform.position += moveVelocity * (animator.GetBool("isRun") ? movePower * 1.7f : movePower) * Time.deltaTime;
        }
    }

    public void PunchEffect(float distance)
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Punch_Hit");
        effect.transform.position = transform.position + new Vector3(isLeftorRight ? -distance : distance, 0, 0);
        effect.transform.rotation = isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
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
    public void GroundEffect()
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("KnockBack_Smoke");
        effect.transform.position = Common.GetBottomPosition(transform);
        effect.transform.rotation = status.isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        effect.SetActive(true);
    }

    private void SkyCheck()
    {
        if (rigid.velocity.y < -3)
        {
            if(!isUnBeat)
                animator.SetBool("isJumping", true);
            skyTime += Math.Abs(rigid.velocity.y) *Time.deltaTime;
        }
        else if (rigid.velocity.y <= 0 && animator.GetBool("isJumping"))
        {
            LandingCheck();
        }
        if (isAir)
        {
            StartCoroutine("Airborne");
        }
        if(isAirborne&&isUnBeat&&rigid.gravityScale<1)
        {
            rigid.gravityScale = 0;
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
        }
    }
    IEnumerator Airborne()
    {
        isAirborne = true;
        isAir = false;
        rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y);
        while (isAirborne)
        {
            yield return new WaitForSeconds(0.35f);
            rigid.gravityScale = 0.1f;
            yield return new WaitForSeconds(0.1f);
            rigid.gravityScale = 0.2f;
            yield return new WaitForSeconds(0.2f);
            rigid.gravityScale = 0.4f;
            yield return new WaitForSeconds(0.2f);
            rigid.gravityScale = 0.7f;
            yield return new WaitForSeconds(0.2f);
            rigid.gravityScale = 0.8f;
            yield return new WaitForSeconds(0.5f);
            isAirborne = false;
        }
        rigid.gravityScale = 1;
        yield return null;
    }
    // 장애물 확인
    void CheckHurdle()
    {
        if (transform.GetChild(0).localRotation.z > 0.6f)
        {
            int layerMask = 1 << 0 | 1 << 22 | 1 << 27 | 1 << 26;
            checkJumpHit2D = Physics2D.Raycast(transform.position + new Vector3(0, -0.5f, 0), isLeftorRight ? Vector2.left : Vector2.right, 0.2f, layerMask);
            Debug.DrawRay(transform.position + new Vector3(0, -0.5f, 0), isLeftorRight ? Vector2.left : Vector2.right, Color.red, 0.2f);
            if (checkJumpHit2D)
            {
                isJumping = true;
                Debugging.Log(name + " 이 장애물 < " + checkJumpHit2D.transform.name + "> 발견함");
            }

            checkTurnHit2D = Physics2D.Raycast(transform.position + new Vector3(isLeftorRight ? -0.5f : 0.5f, 0, 0), Vector2.down, isTrack ? 3f : 1.5f, layerMask);
            Debug.DrawRay(transform.position + new Vector3(isLeftorRight ? -0.5f : 0.5f, 0, 0), isTrack ? Vector2.down * 3 : Vector2.down*1.5f, Color.red, 0.1f);
            if (!checkTurnHit2D)
            {
                isLeftorRight = !isLeftorRight;
            }
        }
    }
    private void LandingCheck()
    {
        if (rigid.velocity.y <= 0 && animator.GetBool("isJumping"))
        {
            int layerMask = 1 << 0 | 1 << 4 | 1 << 22 | 1 << 20 | 1 << 24 | 1 << 26 | 1 << 27;
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.localPosition, Vector2.down, 1.0f, layerMask);
            Debug.DrawRay(transform.localPosition, Vector3.down * 1.0f, Color.blue, 1f);
            foreach (var i in hit)
            {
                if (i.collider && !i.collider.isTrigger)
                {
                    GroundEffect();
                    animator.SetBool("isJumping", false);
                    if (skyTime >= 5f && i.collider.gameObject.layer != 4)
                    {
                        isUnBeat = true;
                        StartCoroutine(UnBeatTime(0));
                        skyTime = 0f;
                    }
                    else
                    {
                        skyTime = 0f;
                    }
                }
            }
        }
    }

    void Climb()
    {
        if (isClimb)
        {
            if (animator.GetBool("isJumping"))
                animator.SetBool("isJumping", false);
            if (!isClimbing)
            {
                if (rigid.constraints == RigidbodyConstraints2D.FreezeRotation)
                    animator.SetTrigger("climbing");
                animator.SetBool("isClimbReady", true);
                rigid.velocity = new Vector2(0, 0);
                rigid.constraints = RigidbodyConstraints2D.FreezeAll;
                rigid.bodyType = RigidbodyType2D.Kinematic;

                this.transform.position = Vector2.Lerp(transform.position, new Vector2(pos.x + ((wallPos.x - pos.x) * 0.2f), wallPos.y + 0.5f), 0.1f);
                if (Math.Abs(transform.position.y - (wallPos.y + 0.5f)) < 0.01f)
                {
                    isClimbing = true;
                }
            }
            //벽오르기
            else
            {
                this.transform.position = Vector2.Lerp(transform.position, new Vector2(wallPos.x, wallPos.y + 1f), 0.1f);
                if (Vector2.Distance(this.transform.position, wallPos + new Vector3(0, 1)) < 0.01f)
                    ClimbEnd();
            }
        }
    }

    void ClimbEnd()
    {
        Debugging.Log("오르기 끝");
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigid.bodyType = RigidbodyType2D.Dynamic;
        isClimb = false;
        isClimbing = false;
        animator.SetBool("isClimbReady", false);
    }

    void Jump()
    {
        if (isJumping&&!isClimb)
        {
            animator.SetBool("isJumping", true);
            animator.SetTrigger("jumping");
            faceAnimator.SetTrigger("Do");
            rigid.velocity = Vector2.zero;
            JumpEffect();
            Vector2 jumpVelocity = new Vector2(isLeftorRight ? -1 : 1, 4*rigid.mass);
            rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);

            isJumping = false;
        }

    }

    void ColorChangeSprites(Color color)
    {
        if(!isDefence)
        {
            var sprites = GetComponentsInChildren<SpriteRenderer>();
            foreach (var sp in sprites)
            {
                sp.color = color;
            }
        }
    }

    IEnumerator TransparentSprite()
    {
        Time.timeScale = 0.3f;
        animator.SetTrigger("deading");
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1;
        yield return new WaitForSeconds(10.0f);
        var sprites = GetComponentsInChildren<SpriteRenderer>();
        
        float alpha = 1.0f;
        while (alpha>=0)
        {
            foreach (var a in sprites)
            {
                if (!a.name.Contains("hair"))
                    a.color = new Color(1, 1, 1, alpha);
            }
            
            alpha -= 0.02f;
            yield return new WaitForEndOfFrame();
        }
        foreach (var a in sprites)
            a.color = new Color(1, 1, 1, 0);
        this.gameObject.SetActive(false);
        yield return null;
        
    }


    #region 물리처리
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead)
            return;
        if ((collision.gameObject.CompareTag("bullet") || collision.gameObject.CompareTag("Boom")) &&!isDead && !isUnBeat)
        {
            Hitted(collision, UnityEngine.Random.Range(status.minAttack, status.maxAttack), 2f, 3f);
        }
        if (collision.CompareTag("attackPoint") && collision.isTrigger && !isDead && !isUnBeat && collision.gameObject != attackPoint.gameObject)
        {
            if (UnityEngine.Random.Range(0, 10) >= 2 || isStunning || isAirborne || isSkill)
            {
                Hitted(collision, UnityEngine.Random.Range(status.minAttack, status.maxAttack), 5f, 3f);
                if (status.weaponType == Common.WeaponType.sword)
                    SwordHitEffect(collision);
                else
                    HitEffect(collision);
            }
            else
            {
                Defence(collision);
                GuardEffect(collision);
            }
        }

        // 벽
        if (collision.CompareTag("Wall") && !isClimb && !isClimbing)
        {
            wallPos = collision.transform.position;
            pos = transform.position;
            if (wallPos.y > transform.position.y)
            {
                if (collision.name.Contains("left"))
                {
                    if (transform.rotation.y != 0)
                        transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else
                {
                    if (transform.rotation.y != -1)
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                isClimb = true;
                rigid.gravityScale = 0;
            }
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead)
            return;
        if (collision.CompareTag("Slope"))
            rigid.gravityScale = 0;

        if (collision.CompareTag("obstacle") && (Math.Abs(collision.GetComponent<Rigidbody2D>().angularVelocity) > 150 || Math.Abs(collision.GetComponent<Rigidbody2D>().velocity.y) > 2||collision.transform.position.y>transform.position.y+0.5f))
        {
            Hitted(collision, (int)collision.GetComponent<Rigidbody2D>().mass, 0f,0f);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isDead)
            return;
        if (collision.CompareTag("Slope"))
            rigid.gravityScale = 1;

        if (collision.gameObject.layer == 0)
        {
            GetComponent<Rigidbody2D>().simulated = true;
        }
        if (collision.CompareTag("Wall"))
        {
            rigid.gravityScale = 1;
        }
    }
    #endregion

    IEnumerator UnBeatTime(int dam)
    {
        OpenHpUI();
        if (skillGuage < 10)
            skillGuage++;
        else
            skillGuage = 10;
        while (isUnBeat)
        {
            ColorChangeSprites(new Color(0.5f, 0.5f, 0.5f));
            yield return new WaitForSeconds(0.1f);
            ColorChangeSprites(new Color(1, 1, 1));
            isUnBeat = false;
        }
        isUnBeat = false;
        if (hp > 0&&!isStunning)
        {
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
            
        else
            yield return null;
        if (isDefence)
            isDefence = false;
        else
        {
            if (dam >= status.maxAttack * 0.95f &&!isStunning&&!isSkill)
            {
                isStun = true;
                DamageFontShow("Stunned", false, true);
                Stunned();
            }
        }
        yield return null;
    }

    IEnumerator OnAttackPoint()
    {
        int count = 0;
        attackPoint.name = attack.ToString();
        attackPoint.gameObject.SetActive(true);
        if (attackPoint)
        {
            attackPoint.GetComponent<AudioSource>().Play();
        }
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
        animator.SetBool("isWait", isWait);
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        while (ct < 1)
        {
            yield return new WaitForSeconds(2.0f);
            if (isAttack)
                isAttack = false;
            ct++;
        }
        isWait = false;
        animator.SetBool("isWait", isWait);
    }

    void StopAttack()
    {
        animator.SetBool("isAttack", false);
        animator.SetInteger("combo", 0);
        if (!isWait && npcMode != NpcMode.Normal)
            StartCoroutine("WaitingForReady");
    }


    public void DamageFontShow(string font, bool isCritical = false, bool isCC = false)
    {
        //GameObject damageUIprefab = ObjectPool.Instance.PopFromPool("damageUI", GameObject.Find("CanvasUI").transform) as GameObject;
        //damageUIprefab.GetComponentInChildren<Text>().text = font.ToString();
        //damageUIprefab.GetComponent<TextDamageController>().isCritical = isCritical;
        //damageUIprefab.GetComponent<TextDamageController>().isCC = isCC;
        //damageUIprefab.transform.position = transform.position + new Vector3(0, 1);
        //damageUIprefab.SetActive(true);
    }
    public void SetWait(bool isWaiting)
    {
        isWait = isWaiting;
    }

    //스킬

     public void SkillUpdate()
    {
        if(skillGuage >= 10)
        {
            isSkill = true;

            OnSkill();

            skillGuage = 0;
        }
    }

    public void OnSkill()
    {
        switch(skillType)
        {
            case SkillType.Near1:
                StartCoroutine("Near1");
                break;
            case SkillType.Near2:
                break;

            case SkillType.Range1:
                break;

            case SkillType.Range2:
                break;
        }
    }

    IEnumerator Near1()
    {
        if(isSkill)
        {
            Debugging.Log("스킬준비");
            RedirectCharacter();

            SpellEffect();
            yield return new WaitForSeconds(1.5f);
            Debugging.Log("스킬시작");
            bool isTrans = true;
            float transTime = 0.0f;
            Explosion_mEffect();

            attackPoint.name = (status.hpFull*0.1f).ToString();
            attackPoint.gameObject.SetActive(true);
            attackPoint.GetComponent<bossAttackPoint>().isSkill = true;
            while (isTrans)
            {
                animator.SetBool("isMoving", true);
                animator.SetBool("isRun", true);
                transform.Translate(new Vector3(-0.05f, 0));
                transTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
                if (transTime > 2.0f)
                    isTrans = false;
            }
            animator.SetBool("isMoving", false);
            animator.SetBool("isRun", false);
            animator.SetTrigger("jumping");
            rigid.AddForce(new Vector2(0, 20), ForceMode2D.Impulse);
            Debugging.Log("스킬마무리");
            yield return new WaitForSeconds(1.0f);
            attackPoint.gameObject.SetActive(false);
            isSkill = false;
        }
        Debugging.Log("스킬끝");
        yield return null;
    }

    private void OpenHpUI()
    {
        GameObject hpUI = ObjectPool.Instance.PopFromPool("hpEnemyUI");
        hpUI.GetComponent<UI_hp>().OpenHpUI(this.gameObject);
        hpUI.gameObject.SetActive(true);
    }
    #region 사운드 매니저

    public void SoundUpdate()
    {
        Sound_HeartBeat();
    }
    public void Sound_HeartBeat()
    {
        if (status.hp < 30)
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
    public void Sound_Jump()
    {
        SoundManager.instance.PlaySingle(AudioClipManager.instance.jump);
    }
    public void Sound_Damage()
    {
        SoundManager.instance.RandomizeSfx(AudioClipManager.instance.damage1, AudioClipManager.instance.damage2);
    }

    public void Sound_Dead()
    {
        SoundManager.instance.RandomizeSfx(AudioClipManager.instance.dead);
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
