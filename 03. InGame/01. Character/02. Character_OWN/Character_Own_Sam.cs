using UnityEngine;
using System.Collections;
using System;
using ATTACK;
using CHARACTER;
using CHARACTER_SPARKY;
using DB_DATA;

namespace CHARACTER_SAM
{
    public enum ATTACK_TYPE { COMBO_1, COMBO_2, COMBO_3, STEAM_BLOW = 10 }
    public enum SKILL { STEAM_BLOW, PULVERIZE, STEAM_BREATH, FULL_BURST }
    public enum AUDIO_SOURCE { SWING, HIT, FOOT_STEP }
}

public class Character_Own_Sam : CharacterCtrl_Own
{
    public enum ATTACK_AREA { LEFT, RIGHT }
    private struct ATTACK_MOVE
    {
        public float m_dist;
        public float m_speed;
        public bool m_isAccelate;
        public float m_accelSpeed;
        public ATTACK_MOVE(float dist, float speed, bool isAccelate, float accelSpeed)
        {
            m_dist = dist;
            m_speed = speed;
            m_isAccelate = isAccelate;
            m_accelSpeed = accelSpeed;
        }
    };
    //스테이터스
    private float[] m_normalAtkPoint;
    //회피관련
    public Animator[] m_dodgeEffect;
    //공격관련
    private bool m_isNormalAttack;
    public AttackArea_Sam[] m_attackArea;
    public Animator[] m_normalAttackEffect;

    //콤보관련
    private int m_battleIdleTime;
    private bool m_isCombo;
    private bool m_checkComboOrder;
    private int m_curNormalAttackCombo;
    private int NORMAL_ATTACK_MAX_COMBO;

    //EP소모량
    private float m_steamBlowEp_Consumtion = 15;
    private float m_pulverizeEp_Consumtion = 12;

    //스팀블로우
    public Animator[] m_steamBlowEffect;
    public GameObject m_steamBlowParticle;
    public AttackArea_SteamBlow m_steamBlowCtrl;
    private bool m_useSteamBlow;
    private bool m_isContact_SteamBlow;
    private float m_steamBlow_ContactAtkPoint = 50f;
    private float m_steamBlow_MoveSpeed = 10f;
    //분쇄
    public AttackArea_Pulverlize m_pulverizeCtrl;
    public GameObject m_pulverizeJumpEffect;
    public Animator m_pulverizeLandingEffect;
    private bool m_usePulverize;
    private float m_pulverize_JumpSpeed = 5f;
    //이펙트 관련
    //발자국 이펙트 발생 위치
    public Transform[] m_footPosition;

    public override void init()
    {
        initType(CHARACTER.TYPE.SAM);
        setCharacterState(CHARACTER.STATE.IDLE);
        initComponent();
    }

    public override void initState(DB_DATA.CHARACTER_INFO status)
    {
        base.initState(status.HLTH_PONT, status.ENRG_PONT, status.DFND_PONT, status.UABL_RGNT, status.HEAL_SCND, status.RUNN_SPED, status.JUMP_SPED, status.GRVT_PONT, status.DDGE_SPED);

        initData();
        initAttackInfo();
    }
    public override void setNormalAttackPoint(int type, int atkPoint) { m_normalAtkPoint[type - 1] = atkPoint; }

    public void initState(float maxHP, float maxEP, float atkPoint_1, float atkPoint_2, float atkPoint_3, float defPoint, float RUT, float HPS, float runSpd, float jSpd, float gravity, float dgSPD)
    {
        base.initState(maxHP, maxEP, defPoint, RUT, HPS, runSpd, jSpd, gravity, dgSPD);
        m_normalAtkPoint[0] = atkPoint_1;
        m_normalAtkPoint[1] = atkPoint_2;
        m_normalAtkPoint[2] = atkPoint_3;

        initData();
        initAttackInfo();
        m_steamBlowCtrl.init(m_character, 50, 2, 4);
        m_pulverizeCtrl.init(m_character, 5, 70);
    }

    public override void initSkillStatus(CHARACTER_SKILL_DATA skillData, bool isActive)
    {
        CHARACTER_SAM.SKILL skillType = (CHARACTER_SAM.SKILL)skillData.SKIL_TYPE;
        m_checkSkillCoolTime.initSkillInfo(skillData.SKIL_TYPE, (int)skillData.COOL_TIME, isActive);

        switch (skillType)
        {
            case CHARACTER_SAM.SKILL.STEAM_BLOW:
                m_steamBlowEp_Consumtion = skillData.USNG_MPNT;
                m_steamBlow_ContactAtkPoint = skillData.CNTT_ATPT;
                m_steamBlowCtrl.init(m_character, skillData.BOMB_ATPT, skillData.ATCK_WDTH, skillData.ATCK_LNTH);
                break;
            case CHARACTER_SAM.SKILL.PULVERIZE :
                m_pulverizeEp_Consumtion = skillData.USNG_MPNT;
                m_pulverizeCtrl.init(m_character, skillData.ATCK_RNGE, skillData.CNTT_ATPT);
                break; 
        }

        Debug.Log("스킬 적용 " + skillType + " " + isActive);
    }

    public new void initComponent()
    {
        base.initComponent();
        m_normalAtkPoint = new float[3];
        for(int i = 0; i < 2; ++i)
        {
            m_attackArea[i].init(this);
            m_attackArea[i].setActive(false);
        }
        m_checkSkillCoolTime = new CheckSkillCoolTime_Sam();
        m_pulverizeJumpEffect.SetActive(false);
    }

    private void initAttackInfo()
    {
        setPossibleAttack(true);

        Skill_First = skill_SteamBlow;
        Skill_Second = skill_Pulverize;
        Skill_Third = Skill_C;
        Skill_Fourth = Skill_D;
    
        m_battleIdleTime = 5;
        m_isNormalAttack = false;
        m_isCombo = false;
        m_checkComboOrder = false;
        m_curNormalAttackCombo = 0;
        NORMAL_ATTACK_MAX_COMBO = 3;

        m_useSteamBlow = false;
        m_isContact_SteamBlow = false;
    }

    // Update is called once per frame
    void Update()
    {
        move();
        if (m_usePulverize == false)
        {
            jumping();
            checkIsGround(0.4f);
        }

        selfDamage();
    }

    public void skill_SteamBlow()
    {
        if (m_usePulverize == true || m_useSteamBlow == true)
            return;

        if (m_checkSkillCoolTime.checkSkillCoolTime((int)CHARACTER_SAM.SKILL.STEAM_BLOW) == true)
        {
            InGameMgr.getInstance().printErrorMessage("아직 사용하실 수 없습니다.", true);
            return;
        }

        if(checkPossibleUseSkill(10, (int)CHARACTER_SAM.SKILL.STEAM_BLOW) == false)
            return;

        if (isJump() == true)
            return;

        if (m_isNormalAttack == true)
            resetAttackState();

        useSkill((int)CHARACTER_SAM.SKILL.STEAM_BLOW);  
        decreaseEP(m_steamBlowEp_Consumtion);

        m_animController.SetTrigger("useSteamBlow");
        ((CheckSkillCoolTime_Sam)m_checkSkillCoolTime).useSteamBlow();
#if SERVER_ON
        InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SAM_STEAM_BLOW_INFO_EM, m_useSteamBlow, m_isContact_SteamBlow);
#endif
    }

    public void skill_SteamBlow_Contact()
    {
        if (m_isContact_SteamBlow == false)
        {
            m_isContact_SteamBlow = true;
            stopAttackMove();
            skill_SteamBlow_ActiveTrigger();
            m_steamBlowEffect[2].SetTrigger("active");
            camShakeEvent(CAM_SHAKE_EVENT.TYPE.SAM_SB, 1f, true, 0.15f);
#if SERVER_ON
            InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SAM_STEAM_BLOW_INFO_EM, m_useSteamBlow, m_isContact_SteamBlow);
#endif
        }
    }

    public void skill_SteamBlow_Reset()
    {
        m_isContact_SteamBlow = false;
        m_useSteamBlow = false;
        StartCoroutine(m_checkSkillCoolTime.printSkillCoolTime((int)CHARACTER_SAM.SKILL.STEAM_BLOW));
#if SERVER_ON
        InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SAM_STEAM_BLOW_INFO_EM, m_useSteamBlow, m_isContact_SteamBlow);
#endif
    }

    private void skill_SteamBlow_ActiveTrigger() { m_steamBlowCtrl.active(); }

    public void skill_Pulverize()
    {
        if (m_usePulverize == true || m_useSteamBlow == true)
            return;
        
        if (m_checkSkillCoolTime.checkSkillCoolTime((int)CHARACTER_SAM.SKILL.PULVERIZE) == true)
        {
            InGameMgr.getInstance().printErrorMessage("아직 사용하실 수 없습니다.", true);
            return;
        }
        
        if (checkPossibleUseSkill(10, (int)CHARACTER_SAM.SKILL.PULVERIZE) == false)
            return;

        if (isJump() == true)
            return;

        if (m_isNormalAttack == true)
            resetAttackState();

        useSkill((int)CHARACTER_SAM.SKILL.PULVERIZE);
        decreaseEP(m_pulverizeEp_Consumtion);

        
        m_animController.SetBool("usePulverize", true);
        ((CheckSkillCoolTime_Sam)m_checkSkillCoolTime).usePulverize();
#if SERVER_ON
        InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CAHRACTER_SAM_PULVERIZE_INFO_EM);
#endif
    }

    public void skill_StartPulverizeJump()
    {
        m_pulverizeJumpEffect.SetActive(true);
        printPunchSound(SOUND_POOL.SAM.ATTACK.SWING, (int)SOUND_POOL.SAM.SKILL.PUVERIZE_START, 1f);
        StartCoroutine("skill_PulverizeJumpRoutine", new ATTACK_MOVE(3, 10, true, -0.4f));
        ObjectPool_Sparky.getInstance().printJumpEffect(transform.position - Vector3.up * 0.2f);
    }
    public void skill_StartPulverizeFall()
    {
        printPunchSound(SOUND_POOL.SAM.ATTACK.SWING, (int)SOUND_POOL.SAM.SKILL.PUVERIZE_LAND, 1f);
        StopCoroutine("skill_PulverizeJumpRoutine");
        StartCoroutine("skill_PulverizeJumpRoutine", new ATTACK_MOVE(-4, 20, true, 0.7f));
    }

    public void skill_Pulverize_ActiveTrigger()
    {
        m_pulverizeCtrl.activeTrigger();
        m_pulverizeLandingEffect.SetTrigger("active");
        camShakeEvent(CAM_SHAKE_EVENT.TYPE.SAM_PUL, 1f, true, 0.15f);
        StartCoroutine(m_checkSkillCoolTime.printSkillCoolTime((int)CHARACTER_SAM.SKILL.PULVERIZE));
    }

    private IEnumerator skill_PulverizeJumpRoutine(ATTACK_MOVE moveInfo)
    {
        Vector3 startPos = m_character.position;
        Vector3 moveVector = Vector3.up * moveInfo.m_dist;
        float moveDist = Vector3.Distance(startPos, m_character.position);
        bool arrive = false;

        Vector3 movePos = Vector3.zero;
        float moveSpeed = moveInfo.m_speed;

        while (arrive == false)
        {
            if (moveInfo.m_isAccelate == true)
            {
                moveSpeed += moveInfo.m_accelSpeed;
                if (moveInfo.m_accelSpeed < 0)
                    if (moveSpeed < 0)
                        break;
            }

            movePos = moveVector.normalized * moveSpeed;
            movePos = transform.parent.TransformDirection(movePos);

            m_controller.Move(movePos * Time.deltaTime);
            moveDist = Vector3.Distance(startPos, m_character.position);

            float dist = Mathf.Abs(moveInfo.m_dist);
            if (moveDist >= dist)
                arrive = true;

            yield return null;
        }
    }

    public void skill_PulverizeReset()
    {
        m_usePulverize = false;
        m_pulverizeJumpEffect.SetActive(false);
        StopCoroutine("skill_PulverizeJumpRoutine");
        StartCoroutine(m_checkSkillCoolTime.printSkillCoolTime((int)CHARACTER_SAM.SKILL.PULVERIZE));
    }

    public void Skill_C()
    {

    }

    public void Skill_D()
    {

    }

    public override void useSkill(int type)
    {
        CHARACTER_SAM.SKILL skillType = (CHARACTER_SAM.SKILL)type;

        switch(skillType)
        {
            case CHARACTER_SAM.SKILL.STEAM_BLOW :
                m_useSteamBlow = true;
                break;
            case CHARACTER_SAM.SKILL.PULVERIZE :
                m_usePulverize = true;
                break;
            case CHARACTER_SAM.SKILL.STEAM_BREATH :
                break;
            case CHARACTER_SAM.SKILL.FULL_BURST :
                break;
        }
        setIsPossibleDodge(false);
        setPossibleJump(false);
        setPossibleMove(false);

        setUseSkill(true);
        setIsBattle(true);
    }

    public override void endSkill()
    {
        if(m_useSteamBlow == true)
            skill_SteamBlow_Reset();

        if (m_usePulverize == true)
            skill_PulverizeReset();

        setUseSkill(false);
    }

    //attack
    public override void attack()
    {
        if (checkPossibleAttack() == false)
            return;

        if (m_curNormalAttackCombo >= NORMAL_ATTACK_MAX_COMBO)
            return;

        setIsBattle(true);
        setPossibleMove(false);
        setPossibleJump(false);
        setBattleIdleAnim(true);

        if (m_isNormalAttack == false)
        {
            m_isNormalAttack = true;
            m_curNormalAttackCombo = 1;
            m_animController.SetBool("isNormalAttack", m_isNormalAttack);
#if SERVER_ON
            InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SAM_NORAML_ATTACK_INFO_EM, true, m_curNormalAttackCombo);
#endif
        }
    }

    public void comboAttack()
    {
        if(m_isNormalAttack == true && m_isCombo == false)
        {
            if (m_curNormalAttackCombo < NORMAL_ATTACK_MAX_COMBO)
            {
                if (m_checkComboOrder == true)
                {
                    m_isCombo = true;
                    setIsBattle(true);
                }
            }
        }
    }

    public void checkComboOrder()
    {
        m_checkComboOrder = true;
        StartCoroutine("checkComboOrderRoutine");
    }
    public void stopComboOrderCheck()
    {
        m_checkComboOrder = false;
        StopCoroutine("checkComboOrderRoutine");
    }
    public IEnumerator checkComboOrderRoutine()
    {
        while(m_checkComboOrder)
        {
            if (m_isCombo == true && m_checkComboOrder == true)
            {
                m_animController.SetBool("normalAttackComboOrder", m_isCombo);
                StopCoroutine("checkComboOrderRoutine");
            }
            yield return null;
        }
    }

    public void activeNormalAttackEffect(int type)
    {
        m_normalAttackEffect[type].SetTrigger("active");
        printPunchSound(SOUND_POOL.SAM.ATTACK.SWING, type, 0.7f);
    }

    public void activeAttackArea(int type)
    {
        try
        {
            if(m_isNormalAttack == true)
                m_attackArea[type].setAttackPoint(m_normalAtkPoint[m_curNormalAttackCombo - 1], m_curNormalAttackCombo);
            else if(m_useSteamBlow == true)
                m_attackArea[type].setAttackPoint(m_steamBlow_ContactAtkPoint, (int)CHARACTER_SAM.ATTACK_TYPE.STEAM_BLOW);
            m_attackArea[type].setActive(true);
        }
        catch
        {
            Debug.Log("activeAttackAreaError : " + m_curNormalAttackCombo);
        }
    }

    public void deActiveAttackArea(int type)
    {
        m_attackArea[type].setActive(false);
        if(m_useSteamBlow == false)
            checkComboOrder();
    }


    public void attackMove(int comboNum)
    {
        float damp = 2f;
        StopCoroutine("attackMoveRoutine");
        switch (comboNum)
        {
            case 1 :
                //StartCoroutine("attackMoveRoutine", new ATTACK_MOVE(0.2f * damp, 1.5f * damp));
                break;
            case 2:
                //StartCoroutine("attackMoveRoutine", new ATTACK_MOVE(0.08f * damp, 1.4f * damp));
                break;
            case 3:
                StartCoroutine("attackMoveRoutine", new ATTACK_MOVE(0.5f * damp, 1.4f * damp, false, 0));
                break;
            case 4:
                StartCoroutine("attackMoveRoutine", new ATTACK_MOVE(0.25f * damp, 0.4f * damp, false, 0));
                break;
            case 5:
                StartCoroutine("attackMoveRoutine", new ATTACK_MOVE(0.5f * damp, 3.5f * damp, false, 0));
                break;
            case 6:
                if(m_isContact_SteamBlow == false)
                    StartCoroutine("attackMoveRoutine", new ATTACK_MOVE(1.5f * damp, 14f * damp, false , 0));
                break;
        }
    }

    private IEnumerator attackMoveRoutine(ATTACK_MOVE moveInfo)
    {
        Vector3 startPos = transform.parent.position;
        Vector3 moveVector = Vector3.forward * moveInfo.m_dist;
        float moveDist = Vector3.Distance(startPos, transform.parent.position);
        bool arrive = false;

        Vector3 movePos = Vector3.zero;
        float moveSpeed = moveInfo.m_speed;

        while (arrive == false)
        {
            if (moveInfo.m_isAccelate == true)
            {
                moveSpeed += moveInfo.m_accelSpeed;
                if (moveInfo.m_accelSpeed < 0)
                    if (moveSpeed < 0)
                        break;
            }

            movePos = moveVector.normalized * moveSpeed;
            movePos = transform.parent.TransformDirection(movePos);

            m_controller.Move(movePos * Time.deltaTime);
            moveDist = Vector3.Distance(startPos, transform.parent.position);

            float dist = Mathf.Abs(moveInfo.m_dist);
            if (moveDist >= dist)
                arrive = true;

            yield return null;
        }
    }

    public void stopAttackMove() { StopCoroutine("attackMoveRoutine"); }

    public override void checkAttackState()
    {
        if (m_isCombo == true)
        {
            if (m_curNormalAttackCombo >= NORMAL_ATTACK_MAX_COMBO)
                resetAttackState();
            else
            {
                m_isCombo = false;
                m_curNormalAttackCombo += 1;
#if SERVER_ON
                InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SAM_NORAML_ATTACK_INFO_EM, true, m_curNormalAttackCombo);
#endif
                m_animController.SetBool("normalAttackComboOrder", m_isCombo);
            }
        }
        else
            resetAttackState();

        Debug.Log(m_curNormalAttackCombo);
    }

    public override void resetAttackState()
    {
        if (m_useSteamBlow == false && m_usePulverize == false && isDown() == false && isDie() == false)
        {
            setPossibleMove(true);
            setPossibleJump(true);
        }

        for (int i = 0; i < m_attackArea.Length; ++i)
            m_attackArea[i].setActive(false);

        stopAttackMove();
        m_isNormalAttack = false;
        m_isCombo = false;
        m_curNormalAttackCombo = 0;
        m_animController.SetBool("isNormalAttack", m_isNormalAttack);
        m_animController.SetBool("normalAttackComboOrder", m_isCombo);
    }

    public override void resetSkillState()
    {
        if(m_useSteamBlow == true)
        {
            m_steamBlowEffect[0].SetBool("isActive", false);
            m_steamBlowParticle.SetActive(false);
            m_useSteamBlow = false;
            stopAttackMove();
        }
		if(m_usePulverize == true)
		{
			m_usePulverize = false;
		}
    }

    public override void resetDamagedInfo()
    {
        setDamagedInfo(false);
        setCharacterStateDisorder(ATTACK.STATE_DISORDER.NONE);
		if (isDown () == false && isDie () == false) 
		{
			if (m_isNormalAttack == false && m_useSteamBlow == false && m_usePulverize == false)
				setPossibleMove (true);
		}
    }

    public override void setLayerWeight(CHARACTER.LAYER layerIndex, float weight)
    {
    }

    public override void smash()
    {
    }
    
    //set Animation
    //-------------------------------------------------------------------------------------
    public override void setDamagedAnim()
    {
        if (m_isNormalAttack == false)
        {
            setDamagedInfo(true);
            setPossibleMove(false);
            m_animController.SetTrigger("Damaged");
        }
    }

    public override void setIsBattle(bool _isBattle)
    {
        if (_isBattle == true)
            m_battleTime = 0;

        if(m_isBattle != _isBattle)
        {
            setBattleIdleAnim(_isBattle);
            if (_isBattle == false)
                recovery(true);
            else
                recovery(false);

            m_isBattle = _isBattle;
        }
    }
    public void setBattleIdleAnim(bool _isBattle)
    {
        m_animController.SetBool("isBattle", _isBattle);
#if SERVER_ON
        InGameServerMgr.getInstance().SendPacket(NET_INGAME.SEND.PACKET_TYPE.REQUEST_CHARACTER_SAM_BATTLE_IDLE_INFO_EM, _isBattle);
#endif
    }

    public override void setAttackAnim(ATTACK.TYPE attackType)
    {
    }

    //PrintEffect
    //-------------------------------------------------------------------------------------
    public void printDodgeEffect()
    {
        switch(m_dodgeDirection)
        {
            case DODGE_DIRECTION.FORWARD :
                m_dodgeEffect[0].SetTrigger("active");
                break;
            case DODGE_DIRECTION.LEFT :
                m_dodgeEffect[1].SetTrigger("active");
                break;
            case DODGE_DIRECTION.RIGHT:
                m_dodgeEffect[2].SetTrigger("active");
                break;
            case DODGE_DIRECTION.BACK :
                break;
        }
    }
    public void removeDodgeEffect()
    {
    }

    public void printSkill_SteamBlowEffect(int step)
    {
        switch(step)
        {
            case 1 :
                m_steamBlowEffect[0].SetBool("isActive", true);
                m_steamBlowParticle.SetActive(true);
                printPunchSound(SOUND_POOL.SAM.ATTACK.SWING,(int)SOUND_POOL.SAM.SKILL.STEAM_BLOW_START, 1f);
                printPunchSound(SOUND_POOL.SAM.ATTACK.HIT, (int)SOUND_POOL.SAM.SKILL.STEAM_BLOW_VOICE, 0.6f);
                break;
            case 2:
                m_steamBlowParticle.SetActive(false);
                m_steamBlowEffect[0].SetBool("isActive", false);
                m_steamBlowEffect[1].SetTrigger("active");
                printPunchSound(SOUND_POOL.SAM.ATTACK.SWING, (int)SOUND_POOL.SAM.SKILL.STEAM_BLOW_HIT, 1f);
                break;
        }
    }

    public void printCreatureHitEffet(Vector3 position, int atkType)
    {
        ObjectPool_Sam.getInstance().printNormalAttackEffect(position, atkType);
        printPunchSound(SOUND_POOL.SAM.ATTACK.HIT, 2 + atkType, 1f);
    }

    public override void printRunningEffect(int type)
    {
        type += 4;
        SOUND_POOL.SPARKY.FOOT_STEP footType = (SOUND_POOL.SPARKY.FOOT_STEP)type;
        switch (footType)
        {
            case SOUND_POOL.SPARKY.FOOT_STEP.LEFT:
                ObjectPool_Sparky.getInstance().printRunningEffect(m_footPosition[0].position, footType);
                break;
            case SOUND_POOL.SPARKY.FOOT_STEP.RIGHT:
                ObjectPool_Sparky.getInstance().printRunningEffect(m_footPosition[1].position, footType);
                break;
        }
        m_audioSource[(int)CHARACTER_SAM.AUDIO_SOURCE.FOOT_STEP].PlayOneShot(SoundMgr.getInstance().getSparkyAudioClip((int)footType), 0.5f);
    }

    public override void printJumpEffect()
    {
        ObjectPool_Sparky.getInstance().printJumpEffect(transform.position - Vector3.up * 0.2f);
    }

    public override void printLandingEffect()
    {
        ObjectPool_Sparky.getInstance().printLandingEffect(transform.position);
        m_audioSource[(int)CHARACTER_SAM.AUDIO_SOURCE.FOOT_STEP].PlayOneShot(SoundMgr.getInstance().getSparkyAudioClip((int)SOUND_POOL.SPARKY.FOOT_STEP.LANDING), 0.5f);
    }

    //PrintSoundEffect
    //-------------------------------------------------------------------------------------
    public void printPunchSound(SOUND_POOL.SAM.ATTACK type, int comboNum, float volume)
    {
        m_audioSource[(int)type].PlayOneShot(SoundMgr.getInstance().getSamAudioClip(comboNum), volume);
    }
    //-------------------------------------------------------------------------------------
    public override bool checkPossibleAttack()
    {
        if (isActive() == false)
            return false;

        if (isPreventAttack() == true)
        {
            InGameMgr.getInstance().printErrorMessage("보급품을 획득해야 무기를 사용할 수 있습니다.", false);
            return false;
        }
        if (isPossibleAttack() == true && isPossibleJump() == true)
        {
            if (isDodge() == false)
                return true;
        }
        return false;
    }
    public override bool checkPossibleUseSkill(float ep_Consumtion, int type)
    {
        if (isActive() == true && isPreventAttack() == false)
        {
            if (isPossibleDodge() == true)
            {
                if (m_isNormalAttack == false)
                {
                    if (isPossibleMove() == false)
                        return false;
                }
                if (usingSkill() == false && m_checkSkillCoolTime.isUsebleSkill(type) == true)
                {
                    if (getCurEP() > ep_Consumtion)
                        return true;
                    else
                        InGameMgr.getInstance().printErrorMessage("에너지가 부족합니다.", false);
                }
            }
        }
        return false;
    }
    //-------------------------------------------------------------------------------------
    //getter
    public override float getCharacterMoveSpeed() { return getMoveSpeed(); }
}
