using UnityEngine;
using System.Collections;
using System;
using DB_DATA;

public class Character_Other_Sam : CharacterCtrl_Other
{
    private bool m_isNormalAttack;
    private int m_curNormalAttackCombo;
    private int MAX_NORMAL_ATTACK_COMBO;
    private bool m_isCombo;
    private float m_battleIdleTime;
    public Animator[] m_normalAttackEffect;

    //스팀블로우 
    public Animator[] m_steamBlowEffect;
    public GameObject m_steamBlowParticle;
    private bool m_useSteamBlow;
    //분쇄
    public GameObject m_pulverizeEffect_Jump;
    public Animator m_pulverizeEffect_Landing;
    private bool m_usePulverize;
    //발자국 이펙트 발생 위치
    public Transform[] m_footPosition;
    //회피관련
    public Animator[] m_dodgeEffect;

    public override void init ()
    {
        initType(CHARACTER.TYPE.SAM);
        initComponent();
    }
    public override void initState(DB_DATA.CHARACTER_INFO status)
    {
        base.initState(status.HLTH_PONT, status.ENRG_PONT, status.DFND_PONT, status.UABL_RGNT, status.HEAL_SCND, status.RUNN_SPED, status.JUMP_SPED, status.GRVT_PONT, status.DDGE_SPED);

        initData();
        initAttackInfo();
    }

    public void initState(float maxHP, float maxEP, float atkPoint, float defPoint, float RUT,float HPS, float runSpd, float jSpd, float gravity, float dgSPD)
    {
        base.initState(maxHP, maxEP, defPoint, RUT, HPS, runSpd, jSpd, gravity, dgSPD);

        initData();
        initAttackInfo();
    }

    public override void initSkillStatus(CHARACTER_SKILL_DATA skillData, bool isActive)
    {
    }

    public new void initComponent()
    {
        base.initComponent();
    }

    public void initAttackInfo()
    {
        m_isNormalAttack = false;
        m_useSteamBlow = false;
        m_battleIdleTime = 5f;
        m_curNormalAttackCombo = 0;
        MAX_NORMAL_ATTACK_COMBO = 3;
    }

    // Update is called once per frame
    void Update()
    {
        interpolateMove();
        interpolateRotation();
        checkIsGround(0.7f);
    }

    public override float getCharacterMoveSpeed() { return getMoveSpeed(); }

    public void skill_SteamBlow(bool useSteamBlow, bool isContact)
    {
        if (useSteamBlow == true)
        {
            if (m_useSteamBlow == false)
            {
                m_useSteamBlow = true;
                m_animController.SetTrigger("useSteamBlow");
            }

            if (isContact == true)
            {
                m_steamBlowEffect[2].SetTrigger("active");
                //InGameMgr.getInstance().getOwnCharacterCtrl().camShakeEvent(CAM_SHAKE_EVENT.TYPE.SAM_SB, 1f, true);
            }
        }
        else
            m_useSteamBlow = false;
    }

    public void skill_Pulverize()
    {
        m_usePulverize = true;
        m_animController.SetTrigger("usePulverize");
    }

    public void skill_PulverizeJump()
    {
        printPunchSound(SOUND_POOL.SAM.ATTACK.SWING, (int)SOUND_POOL.SAM.SKILL.PUVERIZE_START, 1f);
        m_pulverizeEffect_Jump.SetActive(true);
    }
    public void skill_PulverizeLanding()
    {
        printPunchSound(SOUND_POOL.SAM.ATTACK.SWING, (int)SOUND_POOL.SAM.SKILL.PUVERIZE_LAND, 1f);
        m_pulverizeEffect_Landing.SetTrigger("active");
    }

    public void skill_Pulverize_End()
    {
        m_pulverizeEffect_Jump.SetActive(false);
        m_usePulverize = false;
    }

    public override void setAttackInfo(bool attackInfo, bool comboOrder)
    {
        m_isNormalAttack = attackInfo;
        m_isCombo = comboOrder;
        m_animController.SetBool("isNormalAttack", m_isNormalAttack);

        if (m_isNormalAttack == true)
        {
            m_curNormalAttackCombo += 1;
            if(m_isCombo == true)
            {
                m_isCombo = false;
                m_animController.SetTrigger("comboOrder");
            }
        }
        else
        {
            m_animController.ResetTrigger("comboOrder");
            m_curNormalAttackCombo = 0;
        }
    }


    public void setAttackInfo(bool comboOrder, int comboNum)
    {
        if(comboOrder == true)
        {
            m_animController.SetInteger("comboNum", comboNum);
            m_animController.SetTrigger("comboOrder");
        }
    }

    public void activeNormalAttackEffect(int type)
    {
        m_normalAttackEffect[type].SetTrigger("active");
        printPunchSound(SOUND_POOL.SAM.ATTACK.SWING, type, 0.7f);
    }
    
    public void checkComboOrder() { StartCoroutine("checkComboOrderRoutine"); }
    public IEnumerator checkComboOrderRoutine()
    {
        while (true)
        {
            if (m_isCombo == true)
            {
                m_isCombo = false;
                m_animController.SetTrigger("comboOrder");
                StopCoroutine("checkComboOrderRoutine");
            }
            yield return null;
        }
    }

    public override void resetAttackInfo()
    {
        if(m_useSteamBlow == true)
        {
            m_steamBlowEffect[0].SetBool("isActive", false);
            m_steamBlowParticle.SetActive(false);
        }
    }

    //setAnimtion
    //-------------------------------------------------------------------------------------
    public void setBattleIdleAnim(bool _isBattle) { m_animController.SetBool("isBattle", _isBattle); }

    public override void damaged(float damage)
    {
        if (m_useSteamBlow == true || m_isNormalAttack == true)
            return;

        m_animController.SetTrigger("isDamaged");
        //Invoke("resetDamagedInfo", 0.4f);
    }

    public override void setSmashAnim(bool isSmash, int comboNum)
    {
    }
    //PrintEffect
    //-------------------------------------------------------------------------------------
    public void printSkill_SteamBlowEffect(int step)
    {
        switch (step)
        {
            case 1:
                m_steamBlowEffect[0].SetBool("isActive", true);
                m_steamBlowParticle.SetActive(true);
                printPunchSound(SOUND_POOL.SAM.ATTACK.SWING, (int)SOUND_POOL.SAM.SKILL.STEAM_BLOW_START, 1f);
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
        //m_audioSource[(int)CHARACTER_SAM.AUDIO_SOURCE.FOOT_STEP].PlayOneShot(SoundMgr.getInstance().getSparkyAudioClip((int)footType), 0.5f);
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

    public void printDodgeEffect()
    {
        Debug.Log("회피 이펙트 : " + m_dodgeDirection);
        switch (m_dodgeDirection)
        {
            case CHARACTER.DODGE_DIRECTION.FORWARD:
                m_dodgeEffect[0].SetTrigger("active");
                break;
            case CHARACTER.DODGE_DIRECTION.RIGHT:
                m_dodgeEffect[2].SetTrigger("active");
                break;
            case CHARACTER.DODGE_DIRECTION.LEFT:
                m_dodgeEffect[1].SetTrigger("active");
                break;
            case CHARACTER.DODGE_DIRECTION.BACK:
                break;
        }
    }
    //Empty
    //-------------------------------------------------------------------------------------
    public void removeDodgeEffect() { }
    public void attackMove(int comboNum) { }
    public void activeAttackArea(int num) { }
    public void deActiveAttackArea() { }
    //SoundEffect
    //-------------------------------------------------------------------------------------
    public void printPunchSound(SOUND_POOL.SAM.ATTACK type, int comboNum, float volume)
    {
        m_audioSource[(int)type].PlayOneShot(SoundMgr.getInstance().getSamAudioClip(comboNum), volume);
    }
    //-------------------------------------------------------------------------------------
    //setter
    public override void setActive() { setActive(true); }
    public override void setDeActive() { setActive(false); }
    //-------------------------------------------------------------------------------------
}
