using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[RequireComponent(typeof(UserInfo))]
[RequireComponent(typeof(UserInput))]

public abstract class CharacterCtrl_Own : CharacterInfo
{
    //캐릭터 transform
    protected Transform m_character;
    private UserInput m_cameraCtrl;
    private CommunicateToObject m_communication;
    public bool m_damageAnim = false;

    //체력 게이지 관련
    private Character_UIManager m_uiManager;

    //데미지 UI
    private Image m_damageUI;
    private Image m_deathUI;
    //컨트롤 비활성화
    private bool m_lockCameraCtrl = false;
    private bool m_lockDynamicCamera = false;
    private bool m_isActive;

    //전투 관련
    private bool m_preventAttack;
    private bool m_isPossibleAttack;
    protected float m_battleTime;
    protected bool m_isBattle;

    //스킬관련
    protected CheckSkillCoolTime m_checkSkillCoolTime;
    private bool m_useSkill;
    public delegate void Skill();
    public Skill Skill_First;
    public Skill Skill_Second;
    public Skill Skill_Third;
    public Skill Skill_Fourth;

    //사망관련
    private bool m_isDie;
    private Collider m_dieCollider;

    //게임 오브젝트
    protected CharacterController m_controller;
    protected Animator m_animController;

    //이동관련 변수
    private bool m_isPossibleMove;
    private bool m_isMove;
    private Vector3 m_movePos;

    //회피 관련 변수
    private bool m_isDodge;
    private bool m_isPossibleDodge;
    private Vector3 m_dodgeDir;
    protected CHARACTER.DODGE_DIRECTION m_dodgeDirection;

    //점프 관련 변수
    private bool m_isPossibleJump;
    private bool m_startJump;
    private bool m_isJump;
    private bool m_isFall;
    private float m_jumpSpeed;
    private bool m_isStair;
    private Vector3 m_landPos;

    //피격 관련 변수
    private bool m_isDown;
    private bool m_isDamaged;
    private float m_damagedTime;

    //사운드 이펙트
    protected AudioSource[] m_audioSource;

    public abstract void init();
    // Use this for initialization
    public void initData()
    {
        initJumpinfo();
        initMoveInfo();

        m_damagedTime = 0.8f;
        m_uiManager = new Character_UIManager();
        m_uiManager.initHP_Guage();
        m_uiManager.initEP_Guage();
        m_uiManager.initPartsUI();

        m_uiManager.setHP_Guage(getCurHP_Percentage());
        m_uiManager.setEP_Guage(getCurEP_Percentage());

        StartCoroutine(m_uiManager.setGuageAnimation());
        StartCoroutine(m_uiManager.checkPartsNum());
        StartCoroutine(m_uiManager.setPartNumUIAnimation());

        m_preventAttack = false;
        m_isActive = false;
        m_isDamaged = false;
        m_isBattle = false;
        m_useSkill = false;

        m_battleTime = getRegenUnableTime();
        StartCoroutine("checkIsBattle");
    }

    protected void initComponent()
    {
        m_character = transform.parent;

        m_cameraCtrl = GetComponent<UserInput>();
        m_communication = GameObject.Find(OBJECT_NAME.COMUNICATION_OBJECT).GetComponent<CommunicateToObject>();
        m_controller = m_character.GetComponent<CharacterController>();
        m_animController = GetComponent<Animator>();
        m_audioSource = m_character.GetComponents<AudioSource>();
        //m_controller.detectCollisions = false;
        //m_dieCollider = m_character.GetComponent<CapsuleCollider>();
        //m_dieCollider.enabled = false;

        m_damageUI = GameObject.Find(OBJECT_NAME.DAMAGED_UI).GetComponent<Image>();
        m_damageUI.canvasRenderer.SetAlpha(0f);

        m_deathUI = GameObject.Find(OBJECT_NAME.DEATH_UI).GetComponent<Image>();
        m_deathUI.canvasRenderer.SetAlpha(0f);
    }

    private void initJumpinfo()
    {
        m_startJump = false;
        m_isStair = false;
        m_isPossibleJump = true;
        m_isPossibleMove = true;
        m_jumpSpeed = 0.0f;
        m_isJump = false;
        m_landPos = Vector3.zero;
    }

    private void initMoveInfo()
    {
        m_isMove = false;
        m_movePos = Vector3.zero;
        m_dodgeDirection = CHARACTER.DODGE_DIRECTION.STOP;
        m_isDodge = false;
        m_isPossibleDodge = true;
    }

    public void setStartTransform(Transform setTransform)
    {
        if (m_character == null)
            m_character = transform.parent;

        m_character.position = setTransform.position;
        m_character.rotation = setTransform.rotation;
    }

    public abstract void initState(DB_DATA.CHARACTER_INFO status);
    public abstract void setNormalAttackPoint(int type, int atkPoint);
    public void initSkillInfo(int skill_first, int skill_second, int skill_third, int skill_fourth, bool active_first, bool active_second, bool active_third, bool active_fourth)
    {
        m_checkSkillCoolTime.initSkillInfo(skill_first, skill_second, skill_third, skill_fourth, active_first, active_second, active_third, active_fourth);
    }

    public abstract void initSkillStatus(DB_DATA.CHARACTER_SKILL_DATA skillData, bool isActive);

    public void setSkillActive(bool active, int type)
    {
        m_checkSkillCoolTime.setActiveSkill(active, type);
        ProjectMgr.getInstance().setActiveSkill(active, type);
    }

    private IEnumerator checkIsBattle()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (m_isDie == false)
            {
                m_battleTime += 1;
                if (m_battleTime > getRegenUnableTime())
                    setIsBattle(false);
            }
        }
    }

    public void dodge()
    {
        if (checkPossibleDodge() == true)
        {
            m_isDodge = true;
            m_isPossibleDodge = false;
            setDodgeDirection();

            switch (m_dodgeDirection)
            {
                case CHARACTER.DODGE_DIRECTION.STOP:
                case CHARACTER.DODGE_DIRECTION.FORWARD:
                    m_dodgeDirection = CHARACTER.DODGE_DIRECTION.FORWARD;
                    break;

                case CHARACTER.DODGE_DIRECTION.FORWARD_RIGHT:
                case CHARACTER.DODGE_DIRECTION.RIGHT:
                    m_dodgeDirection = CHARACTER.DODGE_DIRECTION.RIGHT;
                    break;

                case CHARACTER.DODGE_DIRECTION.LEFT:
                case CHARACTER.DODGE_DIRECTION.FORWARD_LEFT:
                    m_dodgeDirection = CHARACTER.DODGE_DIRECTION.LEFT;
                    break;

                case CHARACTER.DODGE_DIRECTION.BACK:
                case CHARACTER.DODGE_DIRECTION.BACK_LEFT:
                case CHARACTER.DODGE_DIRECTION.BACK_RIGHT:
                    m_dodgeDirection = CHARACTER.DODGE_DIRECTION.BACK;
                    break;
            }

            resetAttackState();
            setDodgeAnim(true, m_dodgeDirection);
            StopCoroutine("Dodge");
            StartCoroutine("Dodge");
#if SERVER_ON
            InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_DODGE_INFO_EM, m_isDodge, (int)m_dodgeDirection);
#endif
        }
    }

    public void startDodge()
    {
        StopCoroutine("Dodge");
        StartCoroutine("Dodge");
    }

    //애니메이션 프레임에서 호출.
    public void resetDodgeInfo()
    {
		m_isDodge = false;
		setDodgeAnim (false, m_dodgeDirection);
		if (m_isDown == false )
		{
			m_isPossibleDodge = true;
			m_isPossibleJump = true;
			m_isPossibleAttack = true;
		}
    }
    
    public IEnumerator Dodge()
    {
        while(m_isDodge == true)
        {
            Vector3 moveVec = m_dodgeDir * getDodgeSpeed();
            moveVec += Vector3.up * m_jumpSpeed;

            moveVec = m_character.TransformDirection(moveVec);

            m_controller.Move(moveVec * Time.deltaTime);
            yield return null;
        }
    }

    public void setDodgeDirection()
    {
        if (m_movePos.z > 0)
        {
            if (m_movePos.x > 0)
            {
                m_dodgeDirection = CHARACTER.DODGE_DIRECTION.FORWARD_RIGHT;
                m_dodgeDir = Vector3.forward + Vector3.right;
            }
            else if (m_movePos.x < 0)
            {
                m_dodgeDirection = CHARACTER.DODGE_DIRECTION.FORWARD_LEFT;
                m_dodgeDir = Vector3.forward + (-Vector3.right);
            }
            else if (m_movePos.x == 0)
            {
                m_dodgeDirection = CHARACTER.DODGE_DIRECTION.FORWARD;
                m_dodgeDir = Vector3.forward;
            }
        }
        else if (m_movePos.z < 0)
        {
            if (m_movePos.x > 0)
            {
                m_dodgeDirection = CHARACTER.DODGE_DIRECTION.BACK_RIGHT;
                m_dodgeDir = (-Vector3.forward) + Vector3.right;
            }
            else if (m_movePos.x < 0)
            {
                m_dodgeDirection = CHARACTER.DODGE_DIRECTION.BACK_LEFT;
                m_dodgeDir = (-Vector3.forward) + (-Vector3.right);
            }
            else if (m_movePos.x == 0)
            {
                m_dodgeDirection = CHARACTER.DODGE_DIRECTION.BACK;
                m_dodgeDir = -Vector3.forward;
            }
        }
        else if (m_movePos.z == 0)
        {
            if (m_movePos.x > 0)
            {
                m_dodgeDirection = CHARACTER.DODGE_DIRECTION.RIGHT;
                m_dodgeDir = Vector3.right;
            }
            else if (m_movePos.x < 0)
            {
                m_dodgeDirection = CHARACTER.DODGE_DIRECTION.LEFT;
                m_dodgeDir = -Vector3.right;
            }
            else if (m_movePos.x == 0)
            {
                m_dodgeDirection = CHARACTER.DODGE_DIRECTION.STOP;
                m_dodgeDir = Vector3.forward;
            }
        }
        m_dodgeDir.Normalize();
    }

    public void move()
    {
        checkPossibleMove();
        //if (m_animController.GetCurrentAnimatorStateInfo(0).IsName("Locomotion_Run") || m_animController.GetCurrentAnimatorStateInfo(0).IsName("Locomotion_Walk"))
        //{
        m_movePos += Vector3.up * m_jumpSpeed;
        if (m_isStair == true)
            m_movePos += Vector3.up * -10f;

        Vector3 moveVec = m_character.TransformDirection(m_movePos);

       m_controller.Move(moveVec * Time.deltaTime);

        //m_character.transform.position += m_movePos * Time.deltaTime;

        
        //}
    }

    public void setMoveSpeed()
    {
        m_movePos.x *= getCharacterMoveSpeed();
        m_movePos.z *= getCharacterMoveSpeed();
    }

    public void rising()
    {
        m_jumpSpeed = 5f;
    }

    public void riseUp(SEND_DATA data)
    {
        if (m_jumpSpeed < data.fallSpeedCorrection)
            m_jumpSpeed = data.fallSpeedCorrection;

        m_isJump = true;
        m_jumpSpeed += data.force;
        setJumpAnim(true, false);
    }

    public void jump()
    {
        if (checkPossibleJump() == true)
        {
            m_startJump = true;
            m_isJump = true;
            m_isStair = false;
            m_isFall = false;
            setParents(null);
            setPossibleJump(false);
            m_jumpSpeed = base.getJumpSpeed();

            setJumpAnim(true, false);
#if SERVER_ON
            InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_JUMP_EM, m_isJump);
#endif
            Invoke("checkLand", 0.5f);
        }
    }

    public void checkLand() { m_startJump = false; }

    public void jumping()
    {
        if (m_isJump == true)
        {
            if (m_jumpSpeed > -30.0f)
                m_jumpSpeed -= getGravity() * Time.deltaTime;
        }
    }

    public void land(float landHeight)
    {
        if (m_isJump == true)
        {
            Vector3 point = new Vector3(m_character.position.x, m_landPos.y, m_character.position.z);

            setJumpAnim(false, false);

            if (m_animController.GetCurrentAnimatorStateInfo((int)CHARACTER.LAYER.BASE).IsName("Jump_Loop") == false ||
                m_animController.GetCurrentAnimatorStateInfo((int)CHARACTER.LAYER.BASE).IsName("Jump_End") == false)
            {
                setLandPosition();
                /*
#if SERVER_ON
                InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_JUMP_EM, false);
#endif
*/
            }
        }
        else
        {
            GameObject ground = isGround(0.3f);
            if (ground == null)
            {
                if (m_animController.GetCurrentAnimatorStateInfo((int)CHARACTER.LAYER.BASE).IsName("Jump_Loop") == false ||
                    m_animController.GetCurrentAnimatorStateInfo((int)CHARACTER.LAYER.BASE).IsName("Jump_End") == false)
                    setLandPosition();
            }
            if (m_animController.GetBool("isJump") == true)
                setJumpAnim(false, false);
        }
    }

    public void setLandPosition()
    {
        resetJumpInfo();
        /*
        if (m_isStair == true)
            m_character.position = Vector3.Lerp(m_character.position, m_landPos, 10 * Time.deltaTime);
        else
        */
        m_character.position = m_landPos;
    }

    public void resetJumpInfo()
    {
        m_isJump = false;
        m_isFall = false;
        m_jumpSpeed = 0.0f;
    }

    public void fall(float landHeight)
    {
        if (m_isJump == false)
        {
            m_isJump = true;
            m_isFall = true;

            GameObject ground = isGround(landHeight + 0.3f);

            m_isStair = false;
            //더 높게 검사해서 땅이 있는지 확인
            if (ground != null)
            {
                //만약 계단이라면 점프판정x
                if (ground.CompareTag(TAG.STAIR))
                {
                    m_isStair = true;
                }

                //땅이 지정된 높이에 있는지 확인
                ground = isGround(landHeight);
                if(ground != null)
                {
                    //땅이 있으면 땅 위치로 이동.
                    m_isStair = false;
                    setLandPosition();
                }
                else
                    setJumpAnim(true, true);
            }
            else
                setJumpAnim(true, true);
        }
    }

    public void checkIsGround(float landHeight)
    {
        if (m_startJump == false)
        {
            GameObject ground = isGround(landHeight);

            //높이 내에 바닥이 없으면
            if (ground == null)
            {
                fall(landHeight);
                setParents(null);
            }
            else //바닥이 있으면
            {
                if (ground.CompareTag(TAG.STAIR))
                    m_isStair = true;
                else
                    m_isStair = false;

                if (ground.CompareTag(TAG.MOVING_OBJECT) || ground.CompareTag(TAG.ROTATING_OBJECT))
                    setParents(ground.transform);

                else if (m_character.parent != null)
                        setParents(null);

                land(landHeight);
            }
        }
    }

    public GameObject isGround(float dist)
    {
        LayerMask mask = (1 << 9) | (1 << 10) | (1 << 11) | (1 << 12) | (1 << 13) | ( 1 << 16);
        mask = ~mask;
        RaycastHit hit;
        Debug.DrawRay(m_character.position + Vector3.up * 0.2f, -m_character.up * dist, Color.blue);
        Ray ray = new Ray(m_character.position + Vector3.up * 0.2f, -m_character.up);

        if (Physics.Raycast(ray, out hit, dist, mask))
        {
            m_landPos = hit.point;
            return hit.collider.gameObject;
        }

        return null;
    }

    public void setParents(Transform parents) { m_character.parent = parents; }

    public void setGuageFull()
    {
        setHP(getMaxHP());
        setEP(getMaxEP());
        m_uiManager.setHP_Guage(100);
        m_uiManager.setEP_Guage(100);
    }

    public new void increaseEP(float amount)
    {
        base.increaseEP(amount);
        m_uiManager.setEP_Guage(getCurEP_Percentage());
    }

    public new void decreaseEP(float amount)
    {
        base.decreaseEP(amount);
        m_uiManager.setEP_Guage(getCurEP_Percentage());
    }

    public new void increaseHP(float hp)
    {
        base.increaseHP(hp);
        m_uiManager.setHP_Guage(getCurHP_Percentage());
    }
    public new void decreaseHP(float hp)
    {
        base.decreaseHP(hp);
        m_uiManager.setHP_Guage(getCurHP_Percentage());
    }

    public void recovery(bool isStart)
    {
        if(isStart == true)
            StartCoroutine("increaseHP_Guage");
        else
            StopCoroutine("increaseHP_Guage");
    }

    public IEnumerator increaseHP_Guage()
    {
        float callTime = 0.01f;
        float residualHP = getMaxHP() - getCurHP();

        float healingAmount = getHealingPerSecond() * callTime;

        bool isFull = false;

        while (isFull == false)
        {
            increaseHP(healingAmount);

            if (getCurHP() >= getMaxHP())
                isFull = true;

            yield return new WaitForSeconds(callTime);
        }
    }

    public void damaged(ATTACK_INFO attackInfo)
    {
        //if (m_isDodge == true)
        //    return;

        if (isDie() == true)
            return;

        setIsBattle(true);
        attackInfo.damage -= getDefPoint();
        decreaseHP(attackInfo.damage);
        if (m_communication.isRevivalActive() == true)
            m_communication.stopRevival();

        //ObjectPool_Common.getInstance().printDamagedEffect(transform.position + Vector3.up * 0.8f);
        if (getCurHP() <= 0)
        {
            if (m_isDie == false)
            {
                resetAttackState();
                resetSkillState();
                die();
            }
        }
        else
        {
            m_damageAnim = true;
            setCharacterStateDisorder(attackInfo.stateDisorder);
            
            /*
            if (attackInfo.stateDisorder == ATTACK.STATE_DISORDER.NOCK_BACK)
            {
                Vector3 knockBackDir = (transform.position - attackInfo.attackPos).normalized;
                StartCoroutine("knockBackMove", knockBackDir * attackInfo.forcedDist);
                //if(getCharacterType() == CHARACTER.TYPE.SPARKY)
                    resetAttackState();
                setDamagedAnim();
            }*/
            
            if(attackInfo.stateDisorder == ATTACK.STATE_DISORDER.DOWN)
            {
                resetAttackState();
                resetSkillState();
                down();
            }
            else
                Invoke("resetDamagedInfo", m_damagedTime);

            if (m_isJump == false && m_isDodge == false && m_useSkill == false && m_isDown == false)
            {
                setDamagedAnim();
                setPossibleMove(false);
            }

            m_damageUI.canvasRenderer.SetAlpha(1f);
            Invoke("fadeInDamagedUI", 0.5f);
            Invoke("disableDamageAnim", 0.08f);
        }
    }

    public void camShakeEvent(CAM_SHAKE_EVENT.TYPE eventType, float shakeTime, bool decrease, float decreaseTime)
    {
        m_cameraCtrl.playCamShakeEvent(eventType, decrease, decreaseTime);
        Invoke("disableCamShakeEvent", shakeTime);
    }

    public IEnumerator knockBackMove(Vector3 knockBackDist)
    {
        Vector3 moveDir = knockBackDist.normalized;
        float moveDist = 2 * 0.1f;

        for(int i = 0; i < 10; ++i)
        {
            m_controller.Move(moveDir * moveDist);
            yield return new WaitForSeconds(0.003f);
        }

        resetDamagedInfo();
    }

    public void disableDamageAnim() { m_damageAnim = false; }
    public void disableCamShakeEvent() { m_cameraCtrl.stopCamShakeEvent(); }


    public void down()
    {
		if (m_isDie == true)
			return;

        if (m_isDodge == true)
        {
            StopCoroutine("Dodge");
            resetDodgeInfo();
        }
        setPossibleAttack(false);
        setPossibleMove(false);
        setPossibleJump(false);
        setIsPossibleDodge(false);
        m_isDown = true;
        m_animController.SetTrigger("down");
        Debug.Log("down : " + m_isPossibleMove);
#if SERVER_ON
        InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_DOWN_INFO_EM);
#endif
    }

    public void die()
    {
		m_animController.ResetTrigger ("down");
        if (m_isDodge == true)
        {
            StopCoroutine("Dodge");
            resetDodgeInfo();
        }
        m_isActive = false;
        m_isDie = true;

        InGameMgr.getInstance().setDeadCameraTransform();
        setDieAnim();
#if SERVER_ON
        InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_DIE_INFO_EM, true);
#endif

    }

    public void revival()
    {
        setIsBattle(false);
        setGuageFull();
        m_deathUI.CrossFadeAlpha(0f, 1f, false);
        m_animController.SetTrigger("revival");
        m_isDie = false;
        m_isActive = true;
        setPossibleMove(true);
        InGameMgr.getInstance().printRevivalDirection();
    }

    public void fadeInDamagedUI() { m_damageUI.CrossFadeAlpha(0f, 0.5f, false); }
    public void printDeathUI()
    {
        m_deathUI.CrossFadeAlpha(1f, 1.6f, false);

        if (InGameMgr.getInstance().checkAllCharacterDead())
            InGameMgr.getInstance().printGameOverUI(true);
    }
    public void activeDeathUI() { m_deathUI.CrossFadeAlpha(1f, 0.01f, false); }
    public void deActiveDeathUI() { m_deathUI.CrossFadeAlpha(0f, 0.01f, false); }

    public new void increasePartsNum(int number)
    {
        base.increasePartsNum(number);
        m_uiManager.increasePartsNum(number);
    }

    public new void setPartsNum(int num)
    {
        base.setPartsNum(num);
        m_uiManager.increasePartsNum(num);
    }
    public void getItem(CreatureMgr.CreatureDropItemType type, float amount)
    {
        switch(type)
        {
            case CreatureMgr.CreatureDropItemType.PARTS :
                increasePartsNum(1);
                break;
            case CreatureMgr.CreatureDropItemType.ENERGY :
                increaseEP(amount);
                break;
        }
    }

    public float[] getMoveAnim()
    {
        float[] anim = new float[2];
        try
        {
            anim[0] = m_animController.GetFloat("forward");
            anim[1] = m_animController.GetFloat("right");
        }
        catch
        {
            anim[0] = 0f;
            anim[1] = 0f;
        }

        return anim;
    }

    public void resetSkillInfo()
    {
        endSkill();
        setPossibleMove(true);
        setPossibleJump(true);
        setPossibleAttack(true);
        setIsPossibleDodge(true);
    }

    public void resetDownInfo()
    {
        setPossibleAttack(true);
        setPossibleMove(true);
        setPossibleJump(true);
        setIsPossibleDodge(true);
        m_isDown = false;

        resetDamagedInfo();
    }
    //Animation Setter
    //----------------------------------------------------------------------------------//
    public void setJumpAnim(bool isJump, bool isFall)
    {
        m_animController.SetBool("isJump", isJump);
        m_animController.SetBool("isFall", isFall);
    }

    public void setMoveAnim(float forward, float right, bool isMove)
    {
        m_animController.SetBool("isMove", isMove);
        m_animController.SetFloat("forward", forward);
        m_animController.SetFloat("right", right);
    }

    public void setDodgeAnim(bool isDodge, CHARACTER.DODGE_DIRECTION direction)
    {
        if (isDodge == true)
            m_animController.SetTrigger("dodgeOrder");

        m_animController.SetBool("isDodge", m_isDodge);
        m_animController.SetInteger("dodgeDirection", (int)direction);
    }

    public void setDieAnim()
    {
        if(getCharacterType() == CHARACTER.TYPE.SPARKY)
            setLayerWeight(CHARACTER.LAYER.DAMAGED, 0f);

        m_animController.SetTrigger("die");
    }
    public void setAnimTrigger(string name) { m_animController.SetTrigger(name); }
    public void setReviveAnim() { m_animController.SetTrigger("revive"); }
    public void resetSkillAnimation() { m_checkSkillCoolTime.resetSkillAnimation(); }
    //----------------------------------------------------------------------------------//
    //abstract
    public abstract void setDamagedAnim();
    public abstract void resetDamagedInfo();

    public abstract void attack();
    public abstract void smash();

    public abstract void checkAttackState();
    public abstract void resetAttackState();

    public abstract void setAttackAnim(ATTACK.TYPE attackType);

    public abstract void useSkill(int type);
    public abstract void endSkill();
    public abstract void resetSkillState();

    public abstract bool checkPossibleAttack();
    public abstract bool checkPossibleUseSkill(float ep_Consumtion, int type);
    //----------------------------------------------------------------------------------//
    //abstract - effect
    public abstract void printRunningEffect(int type);
    public abstract void printJumpEffect();
    public abstract void printLandingEffect();

    //----------------------------------------------------------------------------------//
    //getter
    public bool isPreventAttack() { return m_preventAttack; }
    public bool isPossibleMove() { return m_isPossibleMove; }
    public bool isPossibleJump() { return m_isPossibleJump; }
    public bool isPossibleDodge() { return m_isPossibleDodge; }
    public bool isMove() { return m_isMove; }
    public bool isJump() { return m_isJump; }
    public bool isDodge() { return m_isDodge; }
    public bool isActive() { return m_isActive; }
    public bool isBattle() { return m_isBattle; }
    public bool isDamaged() { return m_isDamaged; }
    public bool isDown() { return m_isDown; }
    public bool isDie() { return m_isDie; }
    public bool isLockCameraCtrl() { return m_lockCameraCtrl; }
    public bool isLockDynamicCamera() { return m_lockDynamicCamera; }
    public bool isPossibleAttack() { return m_isPossibleAttack; }
    public bool usingSkill() { return m_useSkill; }
    //----------------------------------------------------------------------------------//
    //setter
    public void setPreventAttack(bool isPrevent) { m_preventAttack = isPrevent; }
    public void setMoveVector(Vector3 moveVector) { m_movePos = moveVector; }
    public void setPossibleMove(bool isPossible) { m_isPossibleMove = isPossible; }
    public void setPossibleJump(bool isPossible) { m_isPossibleJump = isPossible; }
    public void setCameraCtrlLock(bool isLock) { m_lockCameraCtrl = isLock; }
    public void setDynamicCameraLock(bool isLock) { m_lockDynamicCamera = isLock; }
    public void setPossibleAttack(bool isPossible) { m_isPossibleAttack = isPossible; }
    public void setIsPossibleDodge(bool isPossible){ m_isPossibleDodge = isPossible; }

    public void setDamagedInfo(bool isDamage) { m_isDamaged = isDamage; }

    public abstract void setIsBattle(bool isBattle);
    public abstract void setLayerWeight(CHARACTER.LAYER layerIndex, float weight);
    public void setIsActive(bool isActive) { m_isActive = isActive; }
    public void setUseSkill(bool use) { m_useSkill = use; }
    //public void resetSkillAnimation() { m_checkSkillCoolTime.resetSkillAnimation(); }
    //----------------------------------------------------------------------------------//
    //sender
    public void deActive() { m_isActive = false; }
    public void active() { m_isActive = true; }
    //----------------------------------------------------------------------------------//
    //checkPossibleAction
    public void checkPossibleMove()
    {
        if (m_isActive == true && m_isPossibleMove == true)
        {
            if (m_movePos == Vector3.zero)
            {
                m_isMove = false;
                setMoveAnim(m_movePos.z, m_movePos.x, m_isMove);
            }
            else
            {
                m_isMove = true;

                if (m_isDodge == true || m_isPossibleMove == false)
                    m_movePos = Vector3.zero;

                setMoveAnim(m_movePos.z, m_movePos.x, m_isMove);
                m_movePos = m_movePos.normalized;
                setMoveSpeed();
            }
        }
        else
        {
            m_movePos = Vector3.zero;
            setMoveAnim(0, 0, false);
        }
    }

    public bool checkPossibleJump()
    {
        if (m_isActive == false || m_isDown == true)
            return false;

        if (m_isPossibleJump == true)
        {
            if (m_isJump == false && m_isDodge == false && m_isDamaged == false && m_useSkill == false)
                return true;
        }
        return false;
    }

    public bool checkPossibleDodge()
    {
        if (m_isActive == false || m_isDown == true)
            return false;

        if (m_isPossibleDodge == true)
        {
            if (m_isJump == false && m_useSkill == false)
                return true;
        }

        return false;
    }

    public void selfDamage()
    {
        /*
        if (Input.GetKeyDown(KeyCode.C))
        {   
            ATTACK_INFO attakcInfo = new ATTACK_INFO();
            attakcInfo.attackPos = Vector3.zero;
            attakcInfo.damage = 50;
            attakcInfo.forcedDist = 0f;
            attakcInfo.stateDisorder = ATTACK.STATE_DISORDER.NONE;
            damaged(attakcInfo); 
            
            resetAttackState();
            resetDamagedInfo();
            resetDodgeInfo();
            resetJumpInfo();
        }*/
    }
}