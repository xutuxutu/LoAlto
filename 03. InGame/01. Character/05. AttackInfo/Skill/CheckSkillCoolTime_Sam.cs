using UnityEngine;
using System.Collections;
using System;

public class CheckSkillCoolTime_Sam : CheckSkillCoolTime
{
    private COOL_TIME m_steamBlowCoolTime;
    private COOL_TIME m_pulverizeCoolTime;
    private COOL_TIME m_steamBreathCoolTime;
    private COOL_TIME m_fullBurstCoolTime;

    public CheckSkillCoolTime_Sam()
    {
        m_steamBlowCoolTime = new COOL_TIME();
        m_pulverizeCoolTime = new COOL_TIME();
        m_steamBreathCoolTime = new COOL_TIME();
        m_fullBurstCoolTime = new COOL_TIME();

        initComponent(OBJECT_NAME.COOL_TIME_STEAM_BLOW_UI, m_steamBlowCoolTime);
        initComponent(OBJECT_NAME.COOL_TIME_PULVERIZE_UI, m_pulverizeCoolTime);
    }

    public override void initSkillInfo(int coolTime_SteamBlow, int coolTime_Pulverize, int coolTime_SteamBreath, int coolTime_FullBurst,
                                        bool active_SteamBlow, bool active_Pulverize, bool active_SteamBreath, bool active_FullBurst)
    {
        setCoolTimeInfo(coolTime_SteamBlow, active_SteamBlow, m_steamBlowCoolTime);
        setCoolTimeInfo(coolTime_Pulverize, active_Pulverize, m_pulverizeCoolTime);
        //setCoolTimeInfo(coolTime_SteamBreath, active_SteamBreath, m_steamBreathCoolTime);
        //setCoolTimeInfo(coolTime_FullBurst, active_FullBurst, m_fullBurstCoolTime);
    }

    public override void initSkillInfo(int type, int coolTime, bool isActive)
    {
        CHARACTER_SAM.SKILL skillType = (CHARACTER_SAM.SKILL)type;

        switch (skillType)
        {
            case CHARACTER_SAM.SKILL.STEAM_BLOW:
                setCoolTimeInfo(coolTime, isActive, m_steamBlowCoolTime);
                break;
            case CHARACTER_SAM.SKILL.PULVERIZE:
                setCoolTimeInfo(coolTime, isActive, m_pulverizeCoolTime);
                break;
            case CHARACTER_SAM.SKILL.STEAM_BREATH:
                //setCoolTimeInfo(coolTime, isActive, m_steamBreathCoolTime);
                break;
            case CHARACTER_SAM.SKILL.FULL_BURST:
                //setCoolTimeInfo(coolTime, isActive, m_fullBurstCoolTime);
                break;
        }
    }

    public override void setActiveSkill(bool active, int type)
    {
        CHARACTER_SAM.SKILL skillType = (CHARACTER_SAM.SKILL)type;

        switch (skillType)
        {
            case CHARACTER_SAM.SKILL.STEAM_BLOW:
                setActiveSkill(true, m_steamBlowCoolTime);
                break;
            case CHARACTER_SAM.SKILL.PULVERIZE:
                setActiveSkill(true, m_pulverizeCoolTime);
                break;
            case CHARACTER_SAM.SKILL.STEAM_BREATH:
                //setActiveSkill(true, m_steamBreathCoolTime);
                break;
            case CHARACTER_SAM.SKILL.FULL_BURST:
                //setActiveSkill(true, m_fullBurstCoolTime);
                break;
        }
    }

    public override IEnumerator printSkillCoolTime(int type)
    {
        CHARACTER_SAM.SKILL skillType = (CHARACTER_SAM.SKILL)type;

        int curTime = 0;
        COOL_TIME skillCoolTime = null;
        switch (skillType)
        {
            case CHARACTER_SAM.SKILL.STEAM_BLOW:
                skillCoolTime = m_steamBlowCoolTime;
                endSteamBlow();
                break;
            case CHARACTER_SAM.SKILL.PULVERIZE:
                skillCoolTime = m_pulverizeCoolTime;
                endPulverize();
                break;
            case CHARACTER_SAM.SKILL.STEAM_BREATH:
                skillCoolTime = m_steamBreathCoolTime;
                break;
            case CHARACTER_SAM.SKILL.FULL_BURST:
                skillCoolTime = m_fullBurstCoolTime;
                break;
        }

        skillCoolTime.m_isCoolTime = true;
        skillCoolTime.m_animator.SetBool("isCoolTime", true);

        while (curTime < skillCoolTime.m_coolTime)
        {
            setCoolTime(skillCoolTime, skillCoolTime.m_coolTime - curTime);
            curTime += 1;
            yield return new WaitForSeconds(1.0f);
        }

        skillCoolTime.m_isCoolTime = false;
        skillCoolTime.m_animator.SetBool("isCoolTime", false);
    }

    public override bool checkSkillCoolTime(int skillNum)
    {
        CHARACTER_SAM.SKILL skillType = (CHARACTER_SAM.SKILL)skillNum;
        switch (skillType)
        {
            case CHARACTER_SAM.SKILL.STEAM_BLOW:
                return m_steamBlowCoolTime.m_isCoolTime;
            case CHARACTER_SAM.SKILL.PULVERIZE:
                return m_pulverizeCoolTime.m_isCoolTime;
            case CHARACTER_SAM.SKILL.STEAM_BREATH:
                return m_steamBreathCoolTime.m_isCoolTime;
            case CHARACTER_SAM.SKILL.FULL_BURST:
                return m_fullBurstCoolTime.m_isCoolTime;
        }
        return false;
    }

    public override void resetSkillAnimation()
    {
        m_steamBlowCoolTime.m_animator.SetBool("isActive", m_steamBlowCoolTime.m_isActive);
        m_pulverizeCoolTime.m_animator.SetBool("isActive", m_pulverizeCoolTime.m_isActive);
        //m_steamBreathCoolTime.m_animator.SetBool("isActive", m_steamBreathCoolTime.m_isActive);
        //m_fullBurstCoolTime.m_animator.SetBool("isActive", m_fullBurstCoolTime.m_isActive);
    }

    public override bool isUsebleSkill(int type)
    {
        CHARACTER_SAM.SKILL skillType = (CHARACTER_SAM.SKILL)type;
        switch (skillType)
        {
            case CHARACTER_SAM.SKILL.STEAM_BLOW:
                return m_steamBlowCoolTime.m_isActive;
            case CHARACTER_SAM.SKILL.PULVERIZE:
                return m_pulverizeCoolTime.m_isActive;
            case CHARACTER_SAM.SKILL.STEAM_BREATH:
                return m_steamBreathCoolTime.m_isActive;
            case CHARACTER_SAM.SKILL.FULL_BURST:
                return m_fullBurstCoolTime.m_isActive;
        }
        return false;
    }


    public void useSteamBlow() { m_steamBlowCoolTime.m_animator.SetBool("isUsing", true); }
    public void endSteamBlow() { m_steamBlowCoolTime.m_animator.SetBool("isUsing", false); }

    public void usePulverize() { m_pulverizeCoolTime.m_animator.SetBool("isUsing", true); }
    public void endPulverize() { m_pulverizeCoolTime.m_animator.SetBool("isUsing", false); }
}
