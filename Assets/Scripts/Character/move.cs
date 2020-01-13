using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class move : MonoBehaviour
{
    public float moveSpeed;
    public float movePower;
    float moveAccPower;
    float moveAttackSpeed = 1;
    float moveDefaultPower = 0.75f;
    public float jumpPower;
    private float jumpTime = 0.3f;
    private float jumpTimeCount = 0.3f;

    public List<AudioClip> FootSoundList = new List<AudioClip>();

    float weaponDelay;
    float delayTime = 0.0f;
    float skillDelayTime = 0.0f;
    float skyTime = 0.0f;

    Rigidbody2D rigid;
    public Animator moveAnimator;
    Animator faceAnimator;

    public GameObject attackPoint;
    public GameObject face;

    private GameObject ropeObject;
    private GameObject me;
    int comboCnt = 0;
    public int MAX_comboCnt;
    public bool isJumping = false;
    public bool isAttack = false;
    public bool isSkill = false;
    public bool isCrouch = false;
    public bool isCrouching = false;
    public bool isCombo = false;
    public bool isClimb = false;
    public bool isClimbing = false;
    public bool isUnBeatTime;
    public bool isWire = false;
    public bool isRope = false;
    public bool isRopeStart = false;
    public bool isReady = false;
    public bool isDash = false;
    public bool isWalk = false;
    public bool isDown = false;
    public bool isSlide = false;
    bool isFrontHit = false;

    float collisionX, collisionY, collisionRotZ;

    Vector3 pos = Vector3.zero;
    Vector2 wallPos = Vector3.zero;
    Vector3 moveVelocity = Vector3.zero;

    void Awake()
    {
        me = this.gameObject;
        rigid = me.GetComponentInChildren<Rigidbody2D>();
       
        if (!moveAnimator)
            moveAnimator = me.GetComponent<Animator>();
        if (face)
            faceAnimator = face.GetComponent<Animator>();

        transform.rotation = Quaternion.Euler(0, 180, 0);

        moveDefaultPower = movePower * 0.28f;
    }
    private void Start()
    {
        EquipWeapon(status.currentWeaponId);
    }

    #region 업데이트
    void Update()
    {
        if (status.isDead || !Common.isStart||status.isPlaying || !Common.isDataLoadSuccess)
            return;
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            moveAnimator.SetBool("isMoving", false);
        }
        if (Input.GetAxisRaw("Vertical") >= 0 && !isRope && !moveAnimator.GetBool("isWater"))
        {
            CrouchEnd();
        }
        else if ((Input.GetAxisRaw("Vertical") == -1 && !isCrouch && !isRope && !moveAnimator.GetBool("isWater")) || isCrouching)
        {
            Crouch();
        }
        if (IsAbleJump())
        {
            isJumping = true;
            faceAnimator.SetTrigger("Do");
            moveAnimator.SetTrigger("jumping");
            JumpEffect();
        }
        if (Input.GetButtonDown("Dash")&&!isRope &&!isClimb && !status.isCtrl && !isClimbing&& !isDown && !isSkill && !moveAnimator.GetBool("isWater"))
        {
            Dash();
        }
        if (IsAbleAttack())
        {
            weaponDelay = CurrentAttackAnimationTime();
            delayTime = 0;
            faceAnimator.SetTrigger("Do");
            comboCnt = 0;
            moveAnimator.SetInteger("combo", comboCnt);
            isAttack = true;
            isCombo = true;
            moveAnimator.SetBool("isAttack", true);
            moveAnimator.SetTrigger("attacking");
        }
        else if (IsAbleAttack(isCombo))
        {
            faceAnimator.SetTrigger("Do");
            delayTime = 0;
            comboCnt++;
            if (status.weaponType == Common.WeaponType.gun)
            {
                moveAnimator.SetInteger("combo", comboCnt % 2);
            }
            else
                moveAnimator.SetInteger("combo", comboCnt);
        }
        else if(IsAbleSkill())
        {
            isSkill = true;
            Debugging.Log("스킬1 발동");
            status.maxAttack *= 2;
            status.minAttack *= 2;
            if (status.weaponType == Common.WeaponType.gun)
            {
                faceAnimator.SetTrigger("Do");
                moveAnimator.SetTrigger("Skill2");
            }
            else
            {
                faceAnimator.SetTrigger("Do");
                moveAnimator.SetTrigger("Skill1");
            }
        }
        else if (Input.GetButtonDown("Change") && !isAttack)
        {
            Sound_Equip();
            ChangeWeapon();
         }
    }
    private void FixedUpdate()
    {
        if (status.isDead || !Common.isStart || status.isPlaying)
        {
            DeadOrAlive();
            return;
        }
        Dead();
        Move();
        Emotion();
        Jump();
        UpRopeControl();
        Push();
        HeadCheck();
        Attack();
        Climb();
        SkyCheck();
        Sliding();
        SoundUpdate();
    }
    #endregion

    #region 물리처리
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (status.isDead || !Common.isStart)
            return;
        // 피격
        if(!isUnBeatTime&&!isDash&&!isDown)
        {
            if (collision.CompareTag("Monster")||gameObject.layer==9)
            {
                isUnBeatTime = true;
                Vector2 attackedVelocity = Vector2.zero;
                if (collision.gameObject.transform.position.x > transform.position.x)
                {
                    if (status.isLeftorRight)
                        isFrontHit = false;
                    else
                        isFrontHit = true;
                    attackedVelocity = new Vector2(-20, 15);
                }
                else
                {
                    if (!status.isLeftorRight)
                        isFrontHit = false;
                    else
                        isFrontHit = true;
                    attackedVelocity = new Vector2(20, 15);
                }
                rigid.AddRelativeForce(attackedVelocity, ForceMode2D.Impulse);
                StartCoroutine(UnBeatTime(1));

            }
            else if (collision.CompareTag("bullet"))
            {
                isUnBeatTime = true;
                Vector2 attackedVelocity = Vector2.zero;
                if (collision.gameObject.transform.position.x > transform.position.x)
                {
                    if (status.isLeftorRight)
                        isFrontHit = false;
                    else
                        isFrontHit = true;
                    attackedVelocity = new Vector2(-5, 3);
                }
                else
                {
                    if (!status.isLeftorRight)
                        isFrontHit = false;
                    else
                        isFrontHit = true;
                    attackedVelocity = new Vector2(5, 3);
                }
                rigid.AddForce(attackedVelocity, ForceMode2D.Impulse);
                StartCoroutine(UnBeatTime(1));
            }
            else if(collision.CompareTag("Boom"))
            {
                isUnBeatTime = true;
                Vector2 attackedVelocity = Vector2.zero;
                if (collision.gameObject.transform.position.x > transform.position.x)
                {
                    if (status.isLeftorRight)
                        isFrontHit = false;
                    else
                        isFrontHit = true;
                    attackedVelocity = new Vector2(-25, 25);
                }
                else
                {
                    if (!status.isLeftorRight)
                        isFrontHit = false;
                    else
                        isFrontHit = true;
                    attackedVelocity = new Vector2(25, 25);
                }
                rigid.AddForce(attackedVelocity, ForceMode2D.Impulse);
                StartCoroutine(UnBeatTime(1));
            }
            else if (collision.CompareTag("attackPointNPC"))
            {
                isUnBeatTime = true;
                StopAllCoroutines();
                Vector2 attackedVelocity = Vector2.zero;
                if (collision.gameObject.transform.position.x > transform.position.x)
                {
                    if (status.isLeftorRight)
                        isFrontHit = false;
                    else
                        isFrontHit = true;
                    attackedVelocity = new Vector2(-5, 3);
                }
                else
                {
                    if (!status.isLeftorRight)
                        isFrontHit = false;
                    else
                        isFrontHit = true;
                    attackedVelocity = new Vector2(5, 3);
                }
                rigid.AddForce(attackedVelocity, ForceMode2D.Impulse);
                StartCoroutine(UnBeatTime(1));
            }
        }
    
        // 벽
        if (collision.CompareTag("Wall") && !isClimb && !isClimbing && !isAttack&&!isDash && !isDown)
        {
            wallPos = collision.transform.position;

            if (wallPos.y > transform.position.y-0.1f)
            {
                if (collision.name.Contains("left"))
                {
                    status.isLeftorRight = false;
                }
                else
                {
                    status.isLeftorRight = true;
                }
                isClimb = true;
                rigid.gravityScale = 0;
            }
        }

        //돈
        if(collision.CompareTag("coin"))
        {
            status.coin++;
            Sound_Coin();
            collision.tag = "Untagged";
            Debugging.Log("코인 : " + status.coin);
            Destroy(collision.gameObject);
        }

        // 움직이는 오브젝트 충돌
        if (collision.gameObject.layer == 27 && (Math.Abs(collision.GetComponent<Rigidbody2D>().velocity.y) > 1 || Math.Abs(collision.GetComponent<Rigidbody2D>().velocity.x) > 2) && transform.position.y < collision.transform.position.y + collision.GetComponentInChildren<SpriteRenderer>(true).bounds.size.y*0.4f)
        {
            Common.isShake = true;
            isUnBeatTime = true;
            StopAllCoroutines();

            Vector2 attackedVelocity = Vector2.zero;
            if (collision.gameObject.transform.position.x > transform.position.x)
            {
                if (status.isLeftorRight)
                    isFrontHit = false;
                else
                    isFrontHit = true;
                attackedVelocity = new Vector2(-10, 5);
            }
            else
            {
                if (!status.isLeftorRight)
                    isFrontHit = false;
                else
                    isFrontHit = true;
                attackedVelocity = new Vector2(10, 5);
            }
            rigid.AddRelativeForce(attackedVelocity, ForceMode2D.Force);
            StartCoroutine(UnBeatTime(status.hpFull));
        }

        //가시덩굴 충돌
        if(collision.CompareTag("thorns"))
        {
            isUnBeatTime = true;
            StopAllCoroutines();
            StartCoroutine(UnBeatTime(status.hpFull));
            Vector2 attackedVelocity = Vector2.zero;
            if (collision.gameObject.transform.position.x > transform.position.x + 0.1f)
                attackedVelocity = new Vector2(-5, 3);
            else
                attackedVelocity = new Vector2(5, 3);
            rigid.AddRelativeForce(attackedVelocity, ForceMode2D.Force);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (status.isDead || !Common.isStart)
            return;

        if (collision.gameObject.CompareTag("Monster") && !collision.isTrigger && !isUnBeatTime)
        {
            isUnBeatTime = true;
            StartCoroutine(UnBeatTime(1));

            Vector2 attackedVelocity = Vector2.zero;
            if (collision.gameObject.transform.position.x > transform.position.x)
                attackedVelocity = new Vector2(-5f, 3f);
            else
                attackedVelocity = new Vector2(5f, 3f);
            rigid.AddForce(attackedVelocity, ForceMode2D.Impulse);
        }
        //로프
        if (collision.CompareTag("Rope"))
        {
            if (!moveAnimator.GetBool("isWire"))
            {
                moveAnimator.SetBool("isWire", true);
            }
            isWire = true;
            if (!collision.name.Contains("right") && !collision.name.Contains("left"))
                collision.GetComponent<Rigidbody2D>().mass = 5;
            collisionY = collision.transform.position.y - 0.5f;
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            rigid.gravityScale = 0;
        }
        if (collision.CompareTag("RopeUp") && Input.GetAxisRaw("Vertical") > 0 && !isUnBeatTime && !isRopeStart && !isClimb&&!isClimbing&&!isDash)
        {
            isRopeStart = true;
            moveAnimator.SetTrigger("ropeing");

            ropeObject = collision.gameObject;
        }
        if (collision.CompareTag("RopeUp") && isRopeStart && !isClimb && !isClimbing && !isDash && !isUnBeatTime && transform.position.y >= collision.transform.position.y - 0.15f && transform.position.
            y < collision.transform.position.y)
        {
            ropeObject = collision.gameObject;
        }
        // 물
        if (collision.gameObject.layer == 4&&collision.transform.localScale.y > 0.15f)
        {
            if (skyTime > 0)
                skyTime = 0;
            if (isClimb || isRope || isRopeStart)
            {
                moveAnimator.SetBool("isWater", false);
            }
            else
            {
                moveAnimator.SetBool("isWater", true);
            }

        }

        // 벽
        if (collision.CompareTag("Wall"))
        {
            rigid.gravityScale = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (status.isDead || !Common.isStart)
            return;
        if (collision.CompareTag("Rope"))
        {
            if (!collision.name.Contains("right") && !collision.name.Contains("left"))
                collision.GetComponent<Rigidbody2D>().mass = 1;
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigid.gravityScale = 1;
            moveAnimator.SetBool("isWire", false);
            isWire = false;
        }
        if (collision.gameObject.layer == 21 && isRope)
        {

            var colliders = Physics2D.OverlapCircleAll(transform.position, 0.3f);
            int cnt = 0;
            foreach (var i in colliders)
            {
                if (i.CompareTag("RopeUp"))
                {
                    cnt++;
                }
            }
            if (cnt == 0)
            {
                rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
                moveAnimator.SetBool("isRope", false);
                isRope = false;
                isRopeStart = false;
                rigid.gravityScale = 1;
            }
        }
        if (collision.CompareTag("Wall"))
        {
            rigid.gravityScale = 1;
        }
        if (collision.gameObject.layer == 4)
        {
            moveAnimator.SetBool("isWater", false);
        }
    }
    #endregion

    public void EquipWeapon(int weaponNumber)
    {
        Item item = ItemSystem.GetItem(weaponNumber);
        status.currentWeaponId = item.id;
        status.weaponType = (Common.WeaponType)item.weapontype;
        status.minAttack = item.minAttack;
        status.maxAttack = item.maxAttack;

        moveAnimator.SetInteger("weaponType", (int)status.weaponType);
        switch (status.weaponType)
        {
            case Common.WeaponType.no:
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(3.5f, 3);
                break; 
            case Common.WeaponType.gun:
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(3, 3);
                break;
            case Common.WeaponType.sword:
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(7, 3);
                break;
            case Common.WeaponType.knife:
                attackPoint.GetComponent<BoxCollider2D>().size = new Vector2(4.5f, 3);
                break;
        }
        Debugging.Log("무기장착 : " + status.currentWeaponId);
    }

    public void SetFalseAllboolean()
    {
        isJumping = false;
        isAttack = false;
        isCrouch = false;
        isCombo = false;
        isClimb = false;
        isClimbing = false;
        isUnBeatTime = false;
        isWire = false;
        isRope = false;
        isRopeStart = false;
        isDash = false;
        isReady = false;
        foreach(var i in moveAnimator.parameters)
        {
            if(i.type==AnimatorControllerParameterType.Bool)
            {
                moveAnimator.SetBool(i.name, false);
            }
        }
    }

    public void ChangeWeapon()
    {
        moveAnimator.SetTrigger("changingWeapon");
        int weaponNum = 0;

        if (status.weapons.Count>0)
        {
            for (int i = 0; i < status.weapons.Count; i++)
            {
                if (status.currentWeaponId == status.weapons[i].id)
                {
                    weaponNum = i;
                    break;
                }
            }
            if(weaponNum+1<status.weapons.Count)
            {
                weaponNum++;
            }
            else
            {
                weaponNum = 0;
            }
            EquipWeapon(status.weapons[weaponNum].id);
        }
    }
    void HeadCheck()
    {
        if(isCrouch)
        {
            string[] exceptLayer = { "Player", "Npc", "Bullet", "IgnorePlayer", "mapObject", "Ignore Raycast", "Item", "UI", "Rope","BackField", "FlockPoint","WarpPoint", "Cameras" };
            int layerMask = ~(LayerMask.GetMask(exceptLayer));
            var headObj = Physics2D.Raycast(transform.position + new Vector3(0, +0.55f, 0), status.isLeftorRight ? Vector2.left : Vector2.right, 0.2f, layerMask);
            Debug.DrawRay(transform.position + new Vector3(0, +0.55f, 0), status.isLeftorRight ? Vector2.left : Vector2.right, Color.yellow, 1f);
            if (headObj)
            {
                isCrouching = true;
            }
            else
            {
                isCrouching = false;
            }
        }
    }
    void Crouch()
    {
        if (Math.Abs(GetComponent<Rigidbody2D>().velocity.y)<0.2f)
        {
            isCrouch = true;
            moveAnimator.SetBool("isCrouching", true);
        }

    }
    void CrouchEnd()
    {
        if (isCrouch&&!isCrouching)
        {
            moveAnimator.SetBool("isCrouching", false);
            isCrouch = false;
        }
    }

    void Emotion()
    {
        if (status.hp <= 1)
            faceAnimator.SetBool("isHurt", true);
        else
            faceAnimator.SetBool("isHurt", false);
    }

    public void FallUp()
    {
        isDown = false;
    }
    void Attack()
    {
        if (isAttack&&!isReady)
        {
            AttackDelay();
        }

        else
        {
            if (attackPoint&&!isSkill)
                attackPoint.SetActive(false);
        }

        if (isSkill && !isReady)
            SkillDelay(1f);
    }
    void AttackDelay()
    {
        delayTime += Time.deltaTime;

        if (delayTime >= weaponDelay)
        {
            ComboReset();
            delayTime = 0;
            isAttack = false;
            moveAnimator.SetBool("isAttack", false);
        }
        else
        {
            // 타겟범위 보조
            if (Common.hitTargetObject()!=null&&Vector3.Distance(Common.hitTargetObject().transform.position,transform.position)<0.5f)
            {
                if(this.transform.position.x > Common.hitTargetObject().transform.position.x)
                {
                    this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(Common.hitTargetObject().transform.position.x+0.5f,this.transform.position.y,0), 0.1f);
                }
                else
                {
                    this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(Common.hitTargetObject().transform.position.x - 0.5f, this.transform.position.y, 0), 0.1f);
                }

            }
        }
    }

    void SkillDelay(float delay)
    {
        skillDelayTime += Time.deltaTime;

        if(skillDelayTime >= delay&&isSkill)
        {
            status.maxAttack /= 2;
            status.minAttack /= 2;
            isSkill = false;
            skillDelayTime = 0;
        }
    }

    bool IsEndAnimation(float percent)
    {
        return moveAnimator.GetCurrentAnimatorStateInfo(0).tagHash.ToString() == "1203776827" && moveAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= percent * 0.01f;
    }
    float CurrentAttackAnimationTime()
    {
        return moveAnimator.GetCurrentAnimatorStateInfo(0).tagHash.ToString() == "1203776827" ? moveAnimator.GetCurrentAnimatorStateInfo(0).length : 0.5f;
    }

    void DeadOrAlive()
    {
        if (status.isDead&&status.hp<1)
        {
            StartCoroutine("Alive");
        }
    }
    IEnumerator Alive()
    {
        status.hp = status.hpFull;

        while (status.isDead)
        {
            yield return new WaitForSeconds(1.0f);
            Camera.main.GetComponent<CameraEffectHandler>().AliveCamera();
            yield return new WaitForSeconds(4.0f);
            status.isDead = false;
            moveAnimator.SetTrigger("aliving");
            faceAnimator.SetBool("isDead", false);
        }

        transform.position = status.checkPoint;
        yield return null;
    }

    void Push()
    {
        if (Input.GetButton("Fire1") && CheckPushObject() != null)
        {
            status.isCtrl = true;
            if (!moveAnimator.GetBool("isPush"))
                moveAnimator.SetTrigger("pushing");

            if (Common.PushedObject.GetComponent<Rigidbody2D>() && Common.PushedObject.GetComponent<Rigidbody2D>().constraints == RigidbodyConstraints2D.FreezeAll)
                Common.PushedObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;


            moveAnimator.SetBool("isPush", true);
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                moveAnimator.SetBool("isMoving", true);
                moveVelocity = Vector3.left;
            }
            else if (Input.GetAxisRaw("Horizontal") > 0)
            {
                moveAnimator.SetBool("isMoving", true);
                moveVelocity = Vector3.right;
            }
            else
            {
                moveAnimator.SetBool("isMoving", false);
                return;
            }
            transform.position += moveVelocity * (moveAccPower * 0.4f) * Time.deltaTime;
            Common.PushedObject.transform.position += moveVelocity * (moveAccPower * 0.4f) * Time.deltaTime;
        }
        else
        {
            DisablePush();
        }
    }

    bool CheckWall()
    {
        int layerMask = 1 << 0 ;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, -0.5f), status.isLeftorRight ? Vector2.left : Vector2.right, 0.4f, layerMask);
        Debug.DrawRay(transform.position+new Vector3(0,-0.5f), status.isLeftorRight ? Vector2.left * 0.4f : Vector2.right*0.4f, Color.red, 0.3f);
        if (hit.collider != null&&!moveAnimator.GetBool("isJumping"))
        {
            moveAnimator.SetBool("isMoving", false);
            return true;
        }
        return false;
    }

    Transform CheckPushObject()
    {
        int layerMask = 1 << 27 | 1 << 22 | 1<<26;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, status.isLeftorRight ? Vector2.left : Vector2.right, 0.6f, layerMask);
        Debug.DrawRay(transform.position, status.isLeftorRight ? Vector2.left : Vector2.right, Color.black, 0.3f);
        if (hit.collider != null&&hit.transform.CompareTag("pushed"))
        {
            if(Common.PushedObject==null)
                Common.PushedObject = hit.transform.gameObject;
            status.isCtrl = true;
            return hit.transform;
        }
        return null;
    }

    void DisablePush()
    {
        status.isCtrl = false;
        moveAnimator.SetBool("isPush", false);
        Common.PushedObject = null;
    }

    void Move()
    {
        if (!isDown && !isClimb &&!isClimbing&& !status.isPlaying&& !isRope && !status.isCtrl && !moveAnimator.GetBool("isWireReady") &&!isSkill&& !isSlide)
        {
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                if(!CheckWall())
                    moveAnimator.SetBool("isMoving", true);
                if (isWire)
                    moveAnimator.SetBool("isWireReady", false);
                status.isLeftorRight = true;
                if (moveAnimator.GetBool("isWater") && Input.GetAxisRaw("Vertical") < 0)
                    moveVelocity = (Vector3.down * 2) + Vector3.left;
                else
                    moveVelocity = Vector3.left;

                if (Common.isCameraOut && Common.restricCamera == Common.RestricCameraType.restricLeft)
                    return;
            }
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                if (!CheckWall())
                    moveAnimator.SetBool("isMoving", true);
                if (isWire)
                    moveAnimator.SetBool("isWireReady", false);
                status.isLeftorRight = false;
                if (moveAnimator.GetBool("isWater") && Input.GetAxisRaw("Vertical") < 0)
                    moveVelocity = (Vector3.down * 2) + Vector3.right;
                else
                    moveVelocity = Vector3.right;

                if (Common.isCameraOut && Common.restricCamera == Common.RestricCameraType.restricRight)
                    return;
            }
            if (Input.GetAxisRaw("Horizontal") == 0)
            {
                moveAccPower = Common.IncrementOrDecrementTowards(moveAccPower, moveDefaultPower, 0.5f, true);
                if (moveAccPower != moveDefaultPower)
                    transform.position += moveVelocity * moveAccPower * Time.deltaTime;
                if (moveAccPower > movePower * 0.8f&&!moveAnimator.GetBool("isWater"))
                    moveAnimator.SetBool("isRun", true);
                else
                    moveAnimator.SetBool("isRun", false);
                return;
            }
            transform.rotation = Quaternion.Euler(0, status.isLeftorRight ? 0 : 180, 0);
            moveAccPower = Common.IncrementOrDecrementTowards(moveAccPower, movePower, 3f);

            if(isWalk)
            {
                if (moveAccPower > movePower * 0.7f)
                    moveAccPower = movePower * 0.7f;
            }

            if (moveAccPower > movePower * 0.8f)
                moveAnimator.SetBool("isRun", true);
            else
                moveAnimator.SetBool("isRun", false);


            if (moveAnimator.GetBool("isAttack") && status.weaponType != Common.WeaponType.gun)
                moveAttackSpeed = 0.1f;
            else if (moveAnimator.GetBool("isAttack") && status.weaponType == Common.WeaponType.gun||isCrouch)
                moveAttackSpeed = 0.7f;
            else
                moveAttackSpeed = 1;

            if (moveAnimator.GetBool("isMoving"))
            {
                pos = this.transform.position;
                pos += (moveVelocity * (moveAccPower * moveAttackSpeed)) * Time.deltaTime;
                pos.y = isWire ? collisionY : pos.y;
            }


            if (isWire)
                this.transform.position = Vector2.Lerp(this.transform.position, pos, 0.5f);
            else
                this.transform.position = pos;
        }
    }

    void Jump()
    {
        if (isJumping && !isClimbing)
        {
            //Physics2D.IgnoreLayerCollision(0, 13, true); //점프시 충돌무시
            if (isWire)
                isWire = false;
            else if (isRope)
            {
                FallingRope();
                rigid.AddRelativeForce(new Vector2(0,jumpPower), ForceMode2D.Impulse);
                isJumping = false;
            }


            if (Input.GetButton("Jump"))
            {
                moveAnimator.SetBool("isJumping", true);
                if (jumpTimeCount>0) 
                {
                    rigid.velocity = Vector2.up * jumpPower;
                    jumpTimeCount -= Time.deltaTime;
                }
                else
                {
                    isJumping = false;
                }
            }
            if (Input.GetButtonUp("Jump"))
                isJumping = false;

        }
    }


    void Dash()
    {
        if(!isDash)
        {
            isDash = true;
            if (moveAnimator.GetBool("isAttack"))
                moveAnimator.SetBool("isAttack", false);
            StartCoroutine("Dashing");
        }
    }

    IEnumerator Dashing()
    {
        Common.SetLayerRecursively(gameObject, 12, 13);
        Camera.main.GetComponent<MotionBlur>().enabled = true;
        while (isDash)
        {
            moveAnimator.SetTrigger("dashing");
            yield return new WaitForSeconds(0.1f);
            rigid.AddForce(new Vector2(status.isLeftorRight ? -20 : 20, 10), ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.9f);
            isDash = false;
        }
        isDash = false;
        Camera.main.GetComponent<MotionBlur>().enabled = false;
        Common.SetLayerRecursively(gameObject, 13, 12);
        yield return null;
    }


    void FallingRope()
    {
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        moveAnimator.SetBool("isRope", false);
        moveAnimator.SetBool("isWireReady", false);
        isRope = false;
        isRopeStart = false;
        rigid.gravityScale = 1;
    }

    void Climb()
    {
        if (isClimb)
        {
            transform.rotation = Quaternion.Euler(0, status.isLeftorRight ? 0 : 180, 0);
            if (moveAnimator.GetBool("isJumping"))
                moveAnimator.SetBool("isJumping", false);
            if (moveAnimator.GetBool("isWireReady"))
                moveAnimator.SetBool("isWireReady", false);
            if (moveAnimator.GetBool("isRope"))
                moveAnimator.SetBool("isRope", false);
            if (!isClimbing)
            {
                if(rigid.constraints == RigidbodyConstraints2D.FreezeRotation)
                    moveAnimator.SetTrigger("climbing");
                moveAnimator.SetBool("isClimbReady", true);
                faceAnimator.SetTrigger("Do");
                rigid.velocity = new Vector2(0, 0);
                rigid.constraints = RigidbodyConstraints2D.FreezeAll;
                rigid.bodyType = RigidbodyType2D.Kinematic;

                this.transform.position = Vector2.Lerp(transform.position, new Vector2(pos.x+((wallPos.x-pos.x)*0.2f), wallPos.y +0.5f), 0.1f);
                if (Math.Abs(transform.position.y - (wallPos.y+ 0.5f))<0.01f)
                {
                    isClimbing = true;
                }
            }
            //벽오르기
            else
            {
                this.transform.position = Vector2.Lerp(transform.position, new Vector2(wallPos.x, wallPos.y +1f), 0.1f);
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
        moveAnimator.SetBool("isClimbReady", false);
    }

    public void Knockback(float kx, float ky, Collider2D collision = null)
    {
        Vector2 attackedVelocity = Vector2.zero;
        if (collision != null)
        {
            if (collision.gameObject.transform.position.x > transform.position.x)
                attackedVelocity = new Vector2(-kx, ky);
            else
                attackedVelocity = new Vector2(kx, ky);
        }
        else
        {
            attackedVelocity = status.isLeftorRight ? new Vector2(kx, ky) : new Vector2(-kx, ky);
        }
        rigid.AddForce(attackedVelocity, ForceMode2D.Impulse);
    }
    //공중체크
    private void SkyCheck()
    {
        if (rigid.velocity.y < -1.5f)
        {
            rigid.gravityScale = 1.5f;

            moveAnimator.SetBool("isJumping", true);
            skyTime += Mathf.Abs(rigid.velocity.y)*Time.deltaTime;
        }
        if (moveAnimator.GetBool("isJumping"))
        {
            LandingCheck();
            if (rigid.velocity.y > 0)
                rigid.gravityScale = 1.5f;
            //if(rigid.velocity.y>0&&!CheckWall())
            //{
            //    Physics2D.IgnoreLayerCollision(13, 0, true);
            //}
            //else
            //{
            //    Physics2D.IgnoreLayerCollision(13, 0, false);
            //}
        }
        else if(rigid.gravityScale>1)
            rigid.gravityScale = 1.0f;

    }
    // 바닥체크
    private void LandingCheck()
    {
        if(rigid.velocity.y<=0&&moveAnimator.GetBool("isJumping"))
        {
            int layerMask = 1 << 0 | 1 << 4 | 1 << 22 | 1 << 20 | 1 << 24 | 1 << 26 | 1 << 27;
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.localPosition, Vector2.down, 1.05f, layerMask);
            Debug.DrawRay(transform.localPosition, Vector3.down*1.05f, Color.blue, 1f);
            foreach(var i in hit)
            {
                if (i.collider && !i.collider.isTrigger)
                {
                    JumpEffect();
                    moveAnimator.SetBool("isJumping", false);
                    if (skyTime >= 5f && i.collider.gameObject.layer != 4)
                    {
                        isUnBeatTime = true;
                        Sound_FootStep();
                        StartCoroutine(UnBeatTime(1));
                    }
                    else
                    {
                        Sound_FootStep();
                    }
                    skyTime = 0f;
                    jumpTimeCount = jumpTime;
                    rigid.gravityScale = 1;
                }
            }
        }
    }
    public void Sliding()
    {
        if (rigid.velocity.y <= 0)
        {
            int layerMask = 1 << 0 | 1 << 4 | 1 << 22 | 1 << 20 | 1 << 24 | 1 << 26 | 1 << 27;
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.localPosition, Vector2.down, 1.05f, layerMask);
            Debug.DrawRay(transform.localPosition, Vector3.down * 1.05f, Color.blue, 1f);
            foreach (var i in hit)
            {
                if (i.collider && !i.collider.isTrigger)
                {
                    if ((i.transform.localRotation.z >= 0.17f&&status.isLeftorRight)||(i.transform.localRotation.z <= -0.17f&&!status.isLeftorRight))
                    {
                        moveVelocity = status.isLeftorRight ? Vector3.left : Vector3.right;
                        transform.position += moveVelocity * 3.0f * Time.deltaTime;
                        JumpEffect();
                        if (!moveAnimator.GetBool("isSlide"))
                        {
                            isSlide = true;
                            moveAnimator.SetTrigger("sliding");
                            moveAnimator.SetBool("isSlide", true);
                        }
                    }
                }
            }
        }
        if(isSlide&&rigid.velocity.y>0)
        {
            isSlide = false;
            moveAnimator.SetBool("isSlide", false);
        }
    }

    public void RecoveryHP(int rhp)
    {
        status.hp = Common.looHpPlus(status.hp, status.hpFull, rhp);
    }
    void ComboReset()
    {
        isCombo = false;
        comboCnt = -1;
        moveAnimator.SetInteger("combo", comboCnt);
    }

    IEnumerator UnBeatTime(int damage)
    {
        int countTime = 0;
        moveAnimator.SetTrigger("heating");
        moveAnimator.SetBool("isHit", true);
        faceAnimator.SetTrigger("Hit");
        FallingRope();
        ComboReset();
        while (countTime < 1)
        {
            status.hp = Common.looMinus(status.hp, damage);
            Debugging.Log("피격당함 >> HP : " + status.hp);
            yield return new WaitForSeconds(2f);
            countTime++;
        }
        moveAnimator.SetBool("isHit", false);
        isUnBeatTime = false;

        yield return null;
    }
    public void Dead()
    {
        if (status.hp <= 0)
        {
            status.isDead = true;
            Common.isStart = false;
            Common.hitTargetName = "";
            SetFalseAllboolean();
            Common.CameraAllEffectOff();
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigid.gravityScale = 1;
            moveAnimator.SetBool("isFrontHit", isFrontHit);
            moveAnimator.SetTrigger("deading");
            faceAnimator.SetBool("isDead", true);
        }
    }

    public void Chat(string chat)
    {
        Common.Chat(chat,null);
    }

    public void Shoot(float speed)
    {
        status.bullet--;
        ShootEffect();
        Sound_Shoot();
        GameObject bullet = ObjectPool.Instance.PopFromPool("Bullet");
        bullet.GetComponent<bulletController>().Target = Common.hitTargetObject() != null? Common.hitTargetObject().transform:null;
        bullet.transform.position = transform.position + new Vector3(0, 0.1f) + transform.right * -1f;
        bullet.transform.rotation = status.isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        bullet.layer = bullet.layer = LayerMask.NameToLayer("Bullet");
        if (speed == 0)
            speed = 25;
        bullet.GetComponent<bulletController>().speed = speed;
        bullet.SetActive(true);
    }


    public void PunchEffect(float distance)
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Punch_Hit");
        effect.transform.position = transform.position + new Vector3(status.isLeftorRight ? -distance : distance, 0, 0);
        effect.transform.rotation = status.isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        effect.SetActive(true);
    }

    public void GroundEffect()
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
        effect.transform.position = transform.position + new Vector3(0,0.2f,0) + transform.right * -1f;
        effect.transform.rotation = status.isLeftorRight ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
        effect.SetActive(true);
    }

    public void RunEffect()
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Run_Smoke");
        effect.transform.position = transform.position + new Vector3(status.isLeftorRight? 0.3f : -0.3f, -0.7f, 0);
        effect.SetActive(true);
    }

    private void JumpEffect()
    {
        GameObject jumpEffect = ObjectPool.Instance.PopFromPool("Jump_Smoke");
        jumpEffect.transform.position = transform.position + new Vector3(0, -0.75f);
        jumpEffect.transform.rotation = status.isLeftorRight ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
        jumpEffect.SetActive(true);
    }
    private void SwordHitEffect()
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Sword_Hit");
        effect.transform.position = transform.position;
        effect.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-10,10));
        effect.SetActive(true);
    }

    private void SwingEffect(float z)
    {
        GameObject effect = ObjectPool.Instance.PopFromPool("Sword_Cut_Medium");
        effect.transform.position = transform.position + new Vector3(status.isLeftorRight ? -1f : 1f, 0.2f, 0);
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

    private void ReloadBullet()
    {
        moveAnimator.SetTrigger("readying");
        status.bullet = 7;
        comboCnt = -1;
        isCombo = false;
        Sound_ReloadPistol();
    }


    private bool IsAbleJump(bool isClimbJump = false)
    {
        if (isDown||isDash)
            return false;
        if(isRope)
        {
            return isClimbJump ? Input.GetButtonDown("Jump") && !moveAnimator.GetBool("isJumping") : Input.GetButtonDown("Jump") && !moveAnimator.GetBool("isJumping") && !moveAnimator.GetBool("isWater") && !isClimb && !isJumping && !isCrouch &&!status.isCtrl&&Input.GetAxisRaw("Vertical")==0;
        }
        else
        {
            return isClimbJump ? Input.GetButtonDown("Jump") && !moveAnimator.GetBool("isJumping") : Input.GetButtonDown("Jump") && !moveAnimator.GetBool("isJumping") && !moveAnimator.GetBool("isWater") && !isClimb && !isCrouch && !isJumping && !status.isCtrl;
        }
    }
    // 스킬공격가능한 상태체크
    private bool IsAbleSkill()
    {
        if (isWire || isClimb || isClimbing || isRope || isCrouching || status.isCtrl || moveAnimator.GetBool("isWater"))
            return false;
        else
        {
            if (Input.GetButtonDown("Fire2")&&!isSkill)
            {
                Debugging.Log("스킬 사용 가능 상태");
                return true;

            }
            else
            {
                return false;
            }
                
        }
    }
    // 공격가능한 상태체크 함수
    private bool IsAbleAttack(bool isComboAttack = false)
    {

        if (isWire || isClimb || isClimbing|| isRope ||isDown || isDash||isCrouching || status.isCtrl || moveAnimator.GetBool("isWater") || IsAbleSkill() || moveAnimator.GetCurrentAnimatorStateInfo(1).IsName("Ready"))
            return false;
        else if(status.weaponType==Common.WeaponType.gun&&status.bullet<=0)
        {
            ReloadBullet();
            return false;
        }
        else
        {

            if (isComboAttack)
            {
                if (moveAnimator.GetBool("isJumping"))
                    return false;
                weaponDelay = CurrentAttackAnimationTime();
                if (Input.GetButton("Fire1") && moveAnimator.GetBool("isAttack") && (delayTime > weaponDelay * 0.5f && isCombo))
                {
                    if (status.weaponType == Common.WeaponType.gun)
                        return true;
                    else if (comboCnt < MAX_comboCnt)
                        return true;
                }
            }
            else
            {
                if (Input.GetButton("Fire1") && !moveAnimator.GetBool("isAttack") && !isCombo)
                    return true;
            }
            return false;
        }

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
        //GetComponentInChildren<weaponTrailer>().TrailOff();
        yield return null;
    }

    void OnSkill()
    {
        //에어본
        string[] exceptLayer = { "Player", "IgnorePlayer", "mapObject", "Ignore Raycast", "Item", "UI", "BackField", "Bullet", "pushed", "Water", "Rope", "MovedObject", "InBuilding", "Cameras" };
        int layerMask = ~(LayerMask.GetMask(exceptLayer));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, status.isLeftorRight ? Vector2.left : Vector2.right, 1f, layerMask);
        Debug.DrawRay(transform.position, status.isLeftorRight ? Vector2.left : Vector2.right, Color.red, 0.3f);
        SkillEffect();
        if (hit.collider!=null)
        {
            
            if(hit.transform.position.x > transform.position.x)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, hit.transform.position + new Vector3(-0.5f, 0, 0), 0.2f);
            }
            else
            {
                this.transform.position = Vector3.Lerp(this.transform.position, hit.transform.position + new Vector3(0.5f, 0, 0), 0.2f);
            }

            OnAirborne(hit, 0, 18);
        }
    }
    void OnLastAttack()
    {
        string[] exceptLayer = { "Player", "IgnorePlayer", "mapObject", "Ignore Raycast", "Item", "UI", "BackField", "Bullet", "pushed", "Water", "Rope", "MovedObject", "InBuilding", "Cameras" };
        int layerMask = ~(LayerMask.GetMask(exceptLayer));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, status.isLeftorRight ? Vector2.left : Vector2.right, 1f, layerMask);
        if (hit.collider != null)
        {
            OnAirborne(hit, 5, 13);
        }
    }

    void OnAirborne(RaycastHit2D col, float x, float y)
    {
        if (col.transform.GetComponent<Rigidbody2D>() != null)
        {
            if (col.transform.GetComponent<npcMove>() != null)
            {
                if (col.transform.GetComponent<npcMove>().isDefence)
                {

                }
                else
                {
                    col.transform.GetComponent<npcMove>().isAir = true;
                    col.transform.GetComponent<npcMove>().isStun = true;
                    col.transform.GetComponent<npcMove>().Stunned();
                    if (x == 0)
                        col.transform.GetComponent<Rigidbody2D>().velocity= new Vector2(0, col.rigidbody.velocity.y);
                    col.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(x, y), ForceMode2D.Impulse);
                }
            }
            else
            {
                if (x == 0)
                    col.transform.GetComponent<Rigidbody2D>().velocity= new Vector2(0, col.rigidbody.velocity.y);
                col.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(x, y), ForceMode2D.Impulse);
            }
        }
    }

    public void UpRopeControl()
    {
        if(!isRope&&isRopeStart)
        {
            if (ropeObject.name.Equals("ladder"))
                moveAnimator.SetBool("isLadder", true);
            else
                moveAnimator.SetBool("isLadder", false);
            moveAnimator.SetBool("isJumping", false);
            skyTime = 0;
            if (status.isLeftorRight)
                this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(ropeObject.transform.position.x + 0.17f, transform.position.y), 0.05f);
            else
                this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(ropeObject.transform.position.x - 0.17f, transform.position.y), 0.05f);

            if (Math.Abs(this.transform.position.x - ropeObject.transform.position.x)<0.25f)
            {
                isRope = true;
            }
        }
        else if (isRope&&isRopeStart)
        {
            moveAnimator.SetBool("isJumping", false);
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            moveAnimator.SetBool("isRope", true);
            collisionY = ropeObject.transform.parent.transform.position.y;
            collisionX = ropeObject.transform.position.x;
            collisionRotZ = ropeObject.transform.localRotation.z;
            bool upKey = Input.GetAxisRaw("Vertical") > 0 && collisionY > transform.position.y + 0.5f;
            bool downKey = Input.GetAxisRaw("Vertical") < 0;
            if (upKey)
            {
                rigid.bodyType = RigidbodyType2D.Kinematic;

                moveAnimator.SetBool("isWireReady", false);
                moveVelocity = Vector2.up;
            }
            else if (downKey)
            {
                rigid.bodyType = RigidbodyType2D.Kinematic;

                moveAnimator.SetBool("isWireReady", false);
                moveVelocity = Vector2.down;
            }
            else
            {
                rigid.bodyType = RigidbodyType2D.Dynamic;
                bool rightKey = Input.GetAxisRaw("Horizontal") > 0;
                bool leftKey = Input.GetAxisRaw("Horizontal") < 0;
                if (rightKey&& Math.Abs(ropeObject.transform.localRotation.z) < 15)
                    ropeObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(100 + (collisionRotZ * 620), 0));
                else if (leftKey && Math.Abs(ropeObject.transform.localRotation.z) < 15)
                    ropeObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(-(100 + (collisionRotZ * 620)), 0));
                if (Math.Abs(collisionRotZ) > 0.15f)
                {
                    ropeObject.GetComponent<Rigidbody2D>().angularVelocity = 0;
                }
                moveVelocity = Vector2.zero;
                moveAnimator.SetBool("isWireReady", true);
            }
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            rigid.gravityScale = 0;

            transform.rotation = Quaternion.Euler(0, status.isLeftorRight ? 0 : 180, status.isLeftorRight ? collisionRotZ * 150 : -collisionRotZ * 150);

            pos = this.transform.position;
            pos.x = status.isLeftorRight ? collisionX + 0.17f : collisionX - 0.17f;
            pos += moveVelocity * movePower * 0.75f * Time.deltaTime;
            this.transform.position = pos;
        }
        else
        {
            isRopeStart = false;
            isRope = false;
            rigid.bodyType = RigidbodyType2D.Dynamic ;
        }
    }
    public void StartGame()
    {
        Common.START_GAME();
    }

    public void AwakeGame()
    {
        Common.AWAKE_GAME();
    }

    #region 컷씬 이벤트 함수
    public void PlayingEventScene(string eventName, bool isPlaying = true)
    {
        if (!status.isPlaying&&eventName!="")
        {
            if(isPlaying)
            {
                SetFalseAllboolean();
                moveAnimator.SetTrigger(eventName);

            }
            status.isPlaying = isPlaying;
        }
    }
    public void PlayingNextEventScene(string eventName)
    {
        if (eventName != "")
        {
            SetFalseAllboolean();
            moveAnimator.SetTrigger(eventName);
            status.isPlaying = true;
        }
    }
    public void EventModeOff()
    {
        Common.CameraAllEffectOff();
    }



    public void SetPlayingStatus()
    {
        Common.SetPlayingStatus(false);
    }

    #endregion

    #region 사운드 매니저

    public void SoundUpdate()
    {
        Sound_HeartBeat();
    }
    public void Sound_HeartBeat()
    {
        if (status.hp <= 1)
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
        SoundManager.instance.RandomizeSfx(AudioClipManager.instance.damage1,AudioClipManager.instance.damage2);
    }
    public void Sound_Pickup()
    {
        SoundManager.instance.RandomizeSfx(AudioClipManager.instance.pickup);
    }

    public void Sound_Equip()
    {
        SoundManager.instance.RandomizeSfx(AudioClipManager.instance.equip);
    }

    public void Sound_Coin()
    {
        SoundManager.instance.RandomizeSfx(AudioClipManager.instance.coin);
    }

    public void Sound_Dead()
    {
        SoundManager.instance.RandomizeSfx(AudioClipManager.instance.dead);
    }

    public void Sound_Kinfe()
    {
        SoundManager.instance.RandomizeSfx(AudioClipManager.instance.knife1,AudioClipManager.instance.knife2);
    }
    public void Sound_FootStep()
    {
        int layerMask = ~(LayerMask.GetMask("Player"));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down,2f, layerMask);
        Debug.DrawRay(transform.position, Vector2.down, Color.red, 0.3f);
        RunEffect();
        if (hit.collider != null)
        {
            if (hit.collider.name.Contains("stone"))
            {
                GetComponent<AudioSource>().clip = FootSoundList[1];
            }
            else
            {
                GetComponent<AudioSource>().clip = FootSoundList[0];
            }
        }
        else
        {
            GetComponent<AudioSource>().clip = FootSoundList[1];
        }
        GetComponent<AudioSource>().Play();
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
